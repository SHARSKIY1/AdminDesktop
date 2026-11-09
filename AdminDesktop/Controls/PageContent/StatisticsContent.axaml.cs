using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminDesktop.Controls.PageContent;

public partial class StatisticsContent : UserControl
{
    private static readonly string[] MonthNames =
    {
        "янв", "фев", "мар", "апр", "май", "июн",
        "июл", "авг", "сен", "окт", "ноя", "дек"
    };

    private static readonly IBrush[] TagBrushes =
    {
        Brush("#8FEA8A"), Brush("#2638E8"), Brush("#7F3FCD"),
        Brush("#267A00"), Brush("#E8C944"), Brush("#2EA4D7"),
        Brush("#E77D58"), Brush("#C65A9B")
    };

    private readonly int[] _newUsers = new int[12];
    private readonly int[] _totalUsers = new int[12];
    private List<(string Name, int Count)> _tags = new();
    private string? _dataNotice;

    public StatisticsContent()
    {
        InitializeComponent();
        LoadStatistics();

        PaymentCanvas.SizeChanged += (_, _) => DrawPaymentChart();
        UsersCanvas.SizeChanged += (_, _) => DrawUsersChart();
        TagsCanvas.SizeChanged += (_, _) => DrawTagsChart();
        ProfitCanvas.SizeChanged += (_, _) => DrawProfitChart();
    }

    private void LoadStatistics()
    {
        try
        {
            using var db = Program.CreateDbContext();
            var registrations = db.Users.Select(user => user.RegistrationDate).ToList();
            var gameTags = db.Games.Select(game => game.Tags).ToList();

            var currentYear = DateTime.Now.Year;
            for (var month = 0; month < 12; month++)
            {
                _newUsers[month] = registrations.Count(date => date.Year == currentYear && date.Month == month + 1);
                _totalUsers[month] = registrations.Count(date => date.Year < currentYear ||
                    (date.Year == currentYear && date.Month <= month + 1));
            }

            _tags = gameTags
                .SelectMany(tags => tags ?? new List<string>())
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .GroupBy(tag => tag.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(group => (Name: group.Key, Count: group.Count()))
                .OrderByDescending(item => item.Count)
                .Take(7)
                .ToList();

            if (registrations.Count == 0)
                _dataNotice = "В базе пока нет регистраций";
        }
        catch (Exception exception)
        {
            _dataNotice = "Нет соединения с БД. Показаны демонстрационные данные.";
            LoadFallbackData();
            System.Diagnostics.Debug.WriteLine($"Statistics data loading failed: {exception.Message}");
        }

        if (_tags.Count == 0)
            _tags = new List<(string Name, int Count)> { ("Нет тегов", 1) };
    }

    private void LoadFallbackData()
    {
        var newUsers = new[] { 2, 5, 3, 4, 1, 7, 18, 12, 4, 2, 6, 8 };
        var total = 0;
        for (var index = 0; index < 12; index++)
        {
            _newUsers[index] = newUsers[index];
            total += newUsers[index];
            _totalUsers[index] = total;
        }
    }

    private void DrawPaymentChart()
    {
        var canvas = PaymentCanvas;
        canvas.Children.Clear();
        var width = canvas.Bounds.Width;
        var height = canvas.Bounds.Height;
        if (width < 20 || height < 20) return;

        var plot = new Rect(10, 8, Math.Max(100, width - 20), Math.Max(75, height - 88));
        AddRectangle(canvas, plot, Brushes.White);
        var values = new[] { 233, 999, 100 };
        var names = new[] { "Card", "SBP", "MoneyStore" };
        var colors = new[] { "#7F3FCD", "#A00000", "#267A00" };
        var max = 1100d;
        var barWidth = Math.Min(34, plot.Width / 7);

        for (var index = 0; index < values.Length; index++)
        {
            var centerX = plot.X + plot.Width * (index + 1) / 4;
            var barHeight = (plot.Height - 28) * values[index] / max;
            AddRectangle(canvas, new Rect(centerX - barWidth / 2, plot.Bottom - barHeight - 8, barWidth, barHeight), Brush(colors[index]));
            AddText(canvas, values[index].ToString(), centerX - 22, plot.Bottom - barHeight - 26, 44, Brushes.Black, 12, true, TextAlignment.Center);
            var legendY = plot.Bottom + 14 + index * 21;
            AddRectangle(canvas, new Rect(plot.X + 16, legendY, 15, 15), Brush(colors[index]));
            AddText(canvas, names[index], plot.X + 38, legendY - 1, Math.Max(65, plot.Width - 48), Brushes.White, 12);
        }
        var paymentNotice = string.IsNullOrWhiteSpace(_dataNotice)
            ? "Демонстрационные данные"
            : "Демо-данные; " + _dataNotice;
        AddText(canvas, paymentNotice, 10, Math.Max(0, height - 17), width - 20, Brush("#F0C674"), 9);
    }

    private void DrawUsersChart()
    {
        var canvas = UsersCanvas;
        canvas.Children.Clear();
        var width = canvas.Bounds.Width;
        var height = canvas.Bounds.Height;
        if (width < 20 || height < 20) return;

        var chart = new Rect(8, 8, Math.Max(100, width - 16), Math.Max(90, height - 72));
        AddRectangle(canvas, chart, Brushes.White);
        var maxValue = Math.Max(5, _totalUsers.Concat(_newUsers).Max());
        var pointsTotal = BuildPoints(chart, _totalUsers, maxValue, 22);
        var pointsNew = BuildPoints(chart, _newUsers, maxValue, 22);
        DrawPolyline(canvas, pointsTotal, Brush("#246477"), 2.2);
        DrawPolyline(canvas, pointsNew, Brush("#8CCF9C"), 2.2);

        for (var index = 0; index < 12; index++)
        {
            AddMarker(canvas, pointsTotal[index], Brush("#246477"));
            AddMarker(canvas, pointsNew[index], Brush("#8CCF9C"));
            AddText(canvas, MonthNames[index], pointsTotal[index].X - 14, chart.Bottom + 2, 28, Brushes.DimGray, 9, false, TextAlignment.Center);
        }

        var legendTop = chart.Bottom + 24;
        AddRectangle(canvas, new Rect(14, legendTop, 14, 14), Brush("#2638E8"));
        AddText(canvas, "Users", 35, legendTop - 1, 75, Brushes.White, 12);
        AddRectangle(canvas, new Rect(14, legendTop + 22, 14, 14), Brush("#8FEA8A"));
        AddText(canvas, "New Users", 35, legendTop + 21, 100, Brushes.White, 12);
        AddNotice(canvas, width, height);
    }

    private void DrawTagsChart()
    {
        var canvas = TagsCanvas;
        canvas.Children.Clear();
        var width = canvas.Bounds.Width;
        var height = canvas.Bounds.Height;
        if (width < 20 || height < 20) return;

        var total = Math.Max(1, _tags.Sum(item => item.Count));
        var center = new Point(Math.Min(width * 0.32, 120), height * 0.49);
        var radius = Math.Min(Math.Min(width * 0.27, height * 0.43), 105);
        var startAngle = -90d;
        var legendX = Math.Min(width * 0.62, center.X + radius + 22);
        var legendY = Math.Max(12, height * 0.18);

        for (var index = 0; index < _tags.Count; index++)
        {
            var sweep = 360d * _tags[index].Count / total;
            var polygon = new Polygon { Fill = TagBrushes[index % TagBrushes.Length], Stroke = Brush("#242C38"), StrokeThickness = 1 };
            polygon.Points.Add(center);
            var segments = Math.Max(3, (int)(sweep / 5));
            for (var step = 0; step <= segments; step++)
            {
                var angle = (startAngle + sweep * step / segments) * Math.PI / 180;
                polygon.Points.Add(new Point(center.X + Math.Cos(angle) * radius, center.Y + Math.Sin(angle) * radius));
            }
            canvas.Children.Add(polygon);

            AddRectangle(canvas, new Rect(legendX, legendY + index * 25, 16, 16), TagBrushes[index % TagBrushes.Length]);
            AddText(canvas, _tags[index].Name, legendX + 23, legendY + index * 25 - 1,
                Math.Max(45, width - legendX - 26), Brushes.White, 11);
            startAngle += sweep;
        }
        AddNotice(canvas, width, height);
    }

    private void DrawProfitChart()
    {
        var canvas = ProfitCanvas;
        canvas.Children.Clear();
        var width = canvas.Bounds.Width;
        var height = canvas.Bounds.Height;
        if (width < 20 || height < 20) return;

        var chart = new Rect(8, 8, Math.Max(100, width - 16), Math.Max(100, height - 48));
        AddRectangle(canvas, chart, Brushes.White);
        var profit = new[] { 357156, 390082, 424912, 433822, 518965, 525043, 590140, 562413, 615655 };
        var sales = new[] { 36400, 43900, 48967, 51800, 74680, 81120, 96560, 98814, 115400 };
        DrawGrid(canvas, chart, 4);
        var profitPoints = BuildPoints(chart, profit, 650000, 12);
        var salesPoints = BuildPoints(chart, sales, 650000, 12);
        DrawPolyline(canvas, profitPoints, Brush("#F0443A"), 2.5);
        DrawPolyline(canvas, salesPoints, Brush("#4387F5"), 2.5);

        for (var index = 0; index < profit.Length; index++)
        {
            AddMarker(canvas, profitPoints[index], Brush("#F0443A"), 4);
            AddMarker(canvas, salesPoints[index], Brush("#4387F5"), 4);
            AddText(canvas, profit[index].ToString("N0"), profitPoints[index].X - 27, profitPoints[index].Y - 18, 55, Brush("#F0443A"), 8, false, TextAlignment.Center);
            AddText(canvas, sales[index].ToString("N0"), salesPoints[index].X - 25, salesPoints[index].Y - 18, 50, Brush("#4387F5"), 8, false, TextAlignment.Center);
        }
        AddText(canvas, "Янв     Фев     Мар     Апр     Май     Июн     Июл     Авг     Сен", chart.X + 5, chart.Bottom + 3, chart.Width - 10, Brushes.White, 9);
        AddText(canvas, "Демонстрационные данные", 10, Math.Max(0, height - 16), width - 20, Brush("#F0C674"), 9);
    }

    private static List<Point> BuildPoints(Rect area, IReadOnlyList<int> values, double maxValue, double topPadding)
    {
        var points = new List<Point>(values.Count);
        var usableHeight = Math.Max(20, area.Height - topPadding - 12);
        for (var index = 0; index < values.Count; index++)
        {
            var x = values.Count == 1 ? area.X : area.X + 8 + (area.Width - 16) * index / (values.Count - 1);
            var y = area.Bottom - 8 - usableHeight * values[index] / Math.Max(1, maxValue);
            points.Add(new Point(x, y));
        }
        return points;
    }

    private static void DrawPolyline(Canvas canvas, IReadOnlyList<Point> points, IBrush color, double thickness)
    {
        for (var index = 1; index < points.Count; index++)
        {
            canvas.Children.Add(new Line
            {
                StartPoint = points[index - 1],
                EndPoint = points[index],
                Stroke = color,
                StrokeThickness = thickness
            });
        }
    }

    private static void DrawGrid(Canvas canvas, Rect chart, int rows)
    {
        for (var index = 0; index <= rows; index++)
        {
            var y = chart.Y + chart.Height * index / rows;
            canvas.Children.Add(new Line
            {
                StartPoint = new Point(chart.X, y),
                EndPoint = new Point(chart.Right, y),
                Stroke = Brush("#D9D9D9"),
                StrokeThickness = 1
            });
        }
    }

    private static void AddMarker(Canvas canvas, Point point, IBrush color, double radius = 3.5)
    {
        var marker = new Ellipse
        {
            Width = radius * 2,
            Height = radius * 2,
            Fill = color
        };
        Canvas.SetLeft(marker, point.X - radius);
        Canvas.SetTop(marker, point.Y - radius);
        canvas.Children.Add(marker);
    }

    private static void AddRectangle(Canvas canvas, Rect rect, IBrush fill)
    {
        var shape = new Rectangle { Width = Math.Max(0, rect.Width), Height = Math.Max(0, rect.Height), Fill = fill };
        Canvas.SetLeft(shape, rect.X);
        Canvas.SetTop(shape, rect.Y);
        canvas.Children.Add(shape);
    }

    private static void AddText(Canvas canvas, string text, double x, double y, double width,
        IBrush foreground, double fontSize, bool bold = false, TextAlignment alignment = TextAlignment.Left)
    {
        var block = new TextBlock
        {
            Text = text,
            Width = Math.Max(1, width),
            FontSize = fontSize,
            FontWeight = bold ? FontWeight.Bold : FontWeight.Normal,
            Foreground = foreground,
            TextAlignment = alignment,
            TextTrimming = TextTrimming.CharacterEllipsis
        };
        Canvas.SetLeft(block, x);
        Canvas.SetTop(block, y);
        canvas.Children.Add(block);
    }

    private void AddNotice(Canvas canvas, double width, double height)
    {
        if (!string.IsNullOrWhiteSpace(_dataNotice))
            AddText(canvas, _dataNotice, 8, Math.Max(0, height - 17), width - 16, Brush("#F0C674"), 9);
    }

    private static SolidColorBrush Brush(string color) => new(Color.Parse(color));
}
