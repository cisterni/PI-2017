open System.Windows.Forms
open System.Drawing

type W2V() =
  let w2v = new Drawing2D.Matrix()
  let v2w = new Drawing2D.Matrix()

  member this.Translate(tx, ty) =
    w2v.Translate(tx, ty)
    v2w.Translate(-tx, -ty, Drawing2D.MatrixOrder.Append)

  member this.Rotate(a) =
    w2v.Rotate(a)
    v2w.Rotate(-a, Drawing2D.MatrixOrder.Append)

  member this.Scale(sx, sy) =
    w2v.Scale(sx, sy)
    v2w.Scale(1.f/sx, 1.f/sy, Drawing2D.MatrixOrder.Append)
  
  member this.W2V with get() = w2v
  member this.V2W with get() = v2w

module Utility =
  let Rect2RectF (r:Rectangle) =
    RectangleF(single r.X, single r.Y, single r.Width, single r.Height)


type PiPaint() as this =
  inherit UserControl()

  let points = ResizeArray<_>([| PointF(0.f, 0.f); PointF(100.f, 100.f) |])
  let R = 5.0f
  let mutable drag = None

  let transform = W2V()

  let handleRect (p:PointF) (r:single) =
    let d = 2.f * r
    RectangleF(p.X - r, p.Y - r, d, d)
  
  let isInHandle (p:PointF) r (p2:PointF)=
    let sqr v = v * v
    let x1, y1 = p2.X - p.X, p2.Y - p.Y
    sqr x1 + sqr y1 <= sqr r

  let transformPoint (m:Drawing2D.Matrix) (p:PointF) =
    let pts = [| p |]
    m.TransformPoints(pts)
    pts.[0]

  let upkey = Rectangle(20, 0, 30, 30)

  do
    this.SetStyle(ControlStyles.AllPaintingInWmPaint ||| ControlStyles.OptimizedDoubleBuffer, true)

  override this.OnKeyDown e =
    match e.KeyCode with
    | Keys.W -> 
      transform.Translate(0.f, 10.f)
      this.Invalidate()
    | Keys.A -> 
      transform.Translate(10.f, 0.f)
      this.Invalidate()
    | Keys.S -> 
      transform.Translate(0.f, -10.f)
      this.Invalidate()
    | Keys.D ->
      transform.Translate(-10.f, 0.f)
      this.Invalidate()
    | Keys.Q ->
      transform.Rotate(10.f)
      this.Invalidate()
    | Keys.E ->
      transform.Rotate(-10.f)
      this.Invalidate()
    | Keys.Z ->
      transform.Scale(1.1f, 1.1f)
      this.Invalidate()
    | Keys.X ->
      transform.Scale(1.f/1.1f, 1.f/1.1f)
      this.Invalidate()
    | _ -> ()

  override this.OnMouseDown e =
    if upkey.Contains(e.Location) then
      transform.Translate(0.f, 10.f)
      this.Invalidate()      
    else
      let mp = transformPoint transform.V2W (PointF(single(e.X), single(e.Y)))
      match (points |> Seq.tryFindIndex(fun p -> isInHandle p R mp)) with
      | Some idx ->
        let p = points.[idx]
        drag <- Some (idx, mp.X - p.X, mp.Y - p.Y)
      
      | None -> 
        points.Add(mp)
        drag <- Some (points.Count - 1, 0.f, 0.f)
        this.Invalidate()
  
  override this.OnMouseMove e =
    let mp = transformPoint transform.V2W (PointF(single(e.X), single(e.Y)))
    match drag with
    | Some (idx, dx, dy) -> 
      points.[idx] <- PointF(mp.X - dx, mp.Y - dy)
      this.Invalidate()
    | None -> ()
  
  override this.OnMouseUp e =
    drag <- None
  
  override this.OnPaint e =
    let g = e.Graphics
    let t = g.Transform

    g.Transform <- transform.W2V
    g.DrawLines(Pens.Black, points.ToArray())
    points |> Seq.iter(fun p ->
      g.DrawEllipse(Pens.Red, handleRect p 5.f)
    )

    g.Transform <- t
    let r = Rectangle(20, 0, 30, 30)
    g.DrawRectangle(Pens.Red, r)
    g.DrawString("W", this.Font, Brushes.Red, r |> Utility.Rect2RectF)


let f = new Form(Dock=DockStyle.Fill,Text= "PiPaint")
// f.Dock <- DockStyle.Fill
// f.Text <- "PiPaint"
let paint = new PiPaint(Dock=DockStyle.Fill)
f.Controls.Add(paint)
f.Show()
//paint.BackColor <- Color.Red