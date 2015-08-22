open Library

type X = GraphicsWindow

let rectWidth, rectHeight = 60, 40
let shift = 50
let across, down = 20, 20
let width, height = across * rectWidth, down * rectHeight
    
do  X.Show()
    X.Width <- width
    X.Height <- height
    let random = System.Random()
    let colors =
        [
            27, 73, 97  // darkest green / black
            52, 105, 79 // dark green
            85, 135, 98 // green
            27,154,137  // blue green
            149,180,102 // light green

            194,69,81   // dark red
            202,67,81   // light red
            236,167,162 // pink

            27,75,160   // dark blue
            37,141,236  // light blue
            100,171,253 // lighter blue / cyan
            
            185,111, 46 // dark orange / brown
            219,156, 76 // orange
            206,186,133 // yellow / dark sand
            225,218,189 // light yellow / sand
            
            183,115,190 // purple
        ]
        |> List.map GraphicsWindow.GetColorFromRGB

    let draw color (x,y,dy) =        
        X.PenColor <- color 
        X.BrushColor <- color
        let e = System.Double.Epsilon
        let x1, x2 = float (x * rectWidth), (float ((x+1) * rectWidth)) - 1.0 + e
        let y1, y2 = float (y * rectHeight + dy), (float ((y+1) * rectHeight)) - 1.0 + e + float dy
        let points = [x1,y1; x2,y1-float shift; x2,y2-float shift; x1,y2]
        X.FillPolygon(points)
        X.DrawPolygon(points)

    for x = 0 to across+1 do
        let mutable last = colors.[0]
        for y = -4 to down + 4 do
            let color =
                if random.Next(8) = 0 then last
                else colors.[random.Next(colors.Length)]
            let dy = ((4-x) % 4) * (shift-rectHeight)
            draw color (x,y,dy)
            last <- color
