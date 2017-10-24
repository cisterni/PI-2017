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

let Rect2RectF (r:Rectangle) =
  RectangleF(single r.X, single r.Y, single r.Width, single r.Height)

type LWControl() =
  let position = PointF()
  let size = SizeF()

  let mutable parent : Control = null

  member this.Parent 
    with get() = parent
    and set(v) = parent <- v

  abstract OnPaint : PaintEventArgs -> unit
  with default this.OnPaint e = ()

type LWContainer() =
  inherit UserControl()
  
  let controlsView = ResizeArray<LWControl>()
  
  member this.LWControls with get() = controlsView

  override this.OnPaint e =
    let g = e.Graphics

    controlsView |> Seq.iter(fun c ->
      c.OnPaint e
    )
