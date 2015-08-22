open Library

GraphicsWindow.Show()
GraphicsWindow.PenColor <- red
for i in 0..5..200 do
    GraphicsWindow.DrawLine(i,0,200-i,200)
    GraphicsWindow.DrawLine(0,i,200,200-i)