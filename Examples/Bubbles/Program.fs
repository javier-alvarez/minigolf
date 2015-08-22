open Library

type Win = GraphicsWindow

Win.Title <- "Bubbles"
let width, height = 1000, 500
Win.Width <- width
Win.Height <- height   
Win.Show() 
Win.BackgroundColor <- black
Win.PenColor <- Colors.Transparent
let rand = System.Random()
let colors = [red; green; blue; yellow]
Win.Update <- fun () ->
    Win.Opacity <- rand.NextDouble() ** 3.0
    Win.BrushColor <- colors.[rand.Next(colors.Length)]
    let x = rand.NextDouble() * float width
    let y = rand.NextDouble() * float height
    let r = 10.0 + rand.NextDouble() * 30.0
    Win.FillEllipse(x-r,y-r,r*2.0,r*2.0)