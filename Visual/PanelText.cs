

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

        canvas.DrawBox(new Rectangle(position.X, position.Y, width, height), ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
                                            new ColoredGlyph(color, Color.Black)));

        canvas.Print(position.X + 1, position.Y, header);

        if(alignType == AlignType.Center)
        {
            canvas.Print(position.X + width / 2 - content.Length / 2, position.Y + 1, content);
        }
        else
        {
            canvas.Print(position.X + 1, position.Y + 1, content);
        }
       

        canvas.IsDirty = true;
    }
}

