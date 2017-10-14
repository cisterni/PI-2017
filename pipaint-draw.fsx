open System.Windows.Forms
open System.Drawing

type PiPaint() as this =
  inherit UserControl()

  let points = ResizeArray<_>()
  let mutable drag = false  

  let handleRect (p:PointF) (r:single) =
    let d = 2.f * r
    RectangleF(p.X - r, p.Y - r, d, d)

  do
    this.SetStyle(ControlStyles.AllPaintingInWmPaint ||| ControlStyles.OptimizedDoubleBuffer, true)
  
  override this.OnMouseDown e =
      drag <- true
      points.Clear() |> ignore
      let p = PointF(single e.X, single e.Y)
      points.Add(p)
  
  override this.OnMouseMove e =
    if drag then
      points.Add(PointF(single e.X, single e.Y))
      this.Invalidate()
  
  override this.OnMouseUp e =
    drag <- false
  
  override this.OnPaint e =
    let g = e.Graphics
    if points.Count > 2 then
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