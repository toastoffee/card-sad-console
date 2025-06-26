

using SadConsole;

namespace CardConsole.Visual;
internal class PanelText
{
    public Point position { get; private set; }

    public string header = "";

    private string m_content = "";
    public string content
    {
        get => m_content;
        set
        {
            m_content = value;
            UpdateVisual();
        }
    }

    public Color color = Color.White;

    public int width, height;

    ScreenSurface canvas;

    public enum AlignType
    {
        Left,
        Center
    }

    public AlignType alignType = AlignType.Left;

    public PanelText(Point position, string header, int width, int height, Color color, ScreenSurface canvas)
    {
        this.position = position;
        this.header = header;
        this.width = width;
        this.height = height;
        this.color = color;
        this.canvas = canvas;
    }

    public void UpdateVisual()
    {

        canvas.DrawBox(new Rectangle(position.X, position.Y, width, height),
        ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(color, Color.Black)));

        canvas.Print(position.X + 1, position.Y, header);

        int maxLineLength = width - 2; // 假设左右各有1个边框
        List<string> lines = new List<string>();

        lines = WordWrap(content, maxLineLength);

        // 输出每一行
        for (int i = 0; i < lines.Count; i++)
        {
            int printX;
            if (alignType == AlignType.Center)
            {
                printX = position.X + width / 2 - lines[i].Length / 2;
            }
            else
            {
                printX = position.X + 1;
            }
            canvas.Print(printX, position.Y + 1 + i, lines[i]);
        }

        canvas.IsDirty = true;
    }

    public static List<string> WordWrap(string text, int maxLineLength)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in words)
        {
            if ((currentLine.Length + word.Length + 1) > maxLineLength)
            {
                lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                if (currentLine.Length > 0)
                    currentLine += " ";
                currentLine += word;
            }
        }
        if (currentLine.Length > 0)
            lines.Add(currentLine);

        return lines;
    }
}

