using Library;

class Program
{    
    static void Main(string[] args)
    {
        GraphicsWindow.Show();
        GraphicsWindow.PenColor = Colors.Red;
        (1000).Times(i =>
        {
            Turtle.Move(6);
            Turtle.Turn(i * 7);
        });
    }
}