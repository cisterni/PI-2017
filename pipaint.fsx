open System.Windows.Forms
open System.Drawing

type PiPaint() as this =
  inherit UserControl()

  let points = ResizeArray<_>([| PointF(0.f, 0.f); PointF(100.f, 100.f) |])
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
    this.SetStyle(ControlStyles.AllPaintingInWmPaint ||| ControlStyles.OptimizedDoubleBuffer, true)
  
  override this.OnMouseDown e =
    // let (|>) x f = f x
    // f (g x) posso scrivere x |> g |> f
    // let add x y = x + y
    // let inc = add 1  
    // Pick correlation
    // Array.tryFindIndex (fun p -> isInHandle p e.X e.Y R)) points
    match (points |> Seq.tryFindIndex(fun p -> isInHandle p e.X e.Y R)) with
    | Some idx ->
      let p = points.[idx]
      drag <- Some (idx, single(e.X) - p.X, single(e.Y) - p.Y)
      
    | None -> 
      let p = PointF(single e.X, single e.Y)
      points.Add(p)
      drag <- Some (points.Count - 1, 0.f, 0.f)
      this.Invalidate()
  
  override this.OnMouseMove e =
    match drag with
    | Some (idx, dx, dy) -> 
      points.[idx] <- PointF(single e.X - dx, single e.Y - dy)
      this.Invalidate()
    | None -> ()
  
  override this.OnMouseUp e =
    drag <- None
  
  override this.OnPaint e =
    let g = e.Graphics
    g.DrawLines(Pens.Black, points.ToArray())
    points |> Seq.iter(fun p ->
      g.DrawEllipse(Pens.Red, handleRect p 5.f)
    )

let f = new Form(Dock=DockStyle.Fill,Text= "PiPaint")
// f.Dock <- DockStyle.Fill
// f.Text <- "PiPaint"
let paint = new PiPaint(Dock=DockStyle.Fill)
f.Controls.Add(paint)
f.Show()
//paint.BackColor <- Color.Red