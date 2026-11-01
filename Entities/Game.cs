using System;
using System.Collections.Generic;
using System.Text;

namespace Entities;

public class Game
{
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public decimal Price { get; set; }
    public string IconPath { get; set; } = string.Empty;
    public string ApkPath { get; set; } = string.Empty;
    public string TagsText => string.Join(", ", Tags);
}

