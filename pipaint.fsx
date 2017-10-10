open System.Windows.Forms
open System.Drawing

type PiPaint() as this =
  inherit UserControl()

  let points = [| PointF(0.f, 0.f); PointF(100.f, 100.f) |]
  let R = 5.0f
  let mutable drag = None
  let handleRect (p:PointF) (r:single) =
    let d = 2.f * r
    RectangleF(p.X - r, p.Y - r, d, d)
  
  let isInHandle (p:PointF) (x:int) (y:int) r=
    let sqr v = v * v
    let x1, y1 = single x - p.X, single y - p.Y
    sqr x1 + sqr y1 <= sqr r

  do
    this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true)
  
  override this.OnMouseDown e =
    // Pick correlation
    match (points |> Array.tryFindIndex(fun p -> isInHandle p e.X e.Y R)) with
    | Some idx -> drag <- Some idx
    | None -> ()
  
  override this.OnMouseMove e =
    match drag with
    | Some idx -> 
      points.[idx] <- PointF(single e.X, single e.Y)
      this.Invalidate()
    | None -> ()
  
  override this.OnMouseUp e =
    drag <- None
  
  override this.OnPaint e =
    let g = e.Graphics
    let p1, p2 = points.[0], points.[1]
    g.DrawLine(Pens.Black, p1, p2)
    g.DrawEllipse(Pens.Red, handleRect p1 5.f)
    g.DrawEllipse(Pens.Red, handleRect p2 5.f)

let f = new Form(Dock=DockStyle.Fill,Text= "PiPaint")
// f.Dock <- DockStyle.Fill
// f.Text <- "PiPaint"
let paint = new PiPaint(Dock=DockStyle.Fill)
f.Controls.Add(paint)
f.Show()
