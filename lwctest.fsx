#load "lwc.fsx"

open System.Windows.Forms
open System.Drawing

open Lwc

type PIButton() =
  inherit LWControl()

  let mutable text = ""

  member this.Text
    with get() = text
    and set(v) = text <- v

  override this.OnPaint e =
    let parent = this.Parent
    let g = e.Graphics
    let r = RectangleF(this.Position, this.Size) |> RectF2Rect
    g.DrawRectangle(Pens.Red, r)
    let ssz = g.MeasureString(text, parent.Font)
    let p = this.Position
    let sz = this.Size
    let sx, sy = p.X + (sz.Width - ssz.Width) / 2.f, p.Y + (sz.Height - ssz.Height) / 2.f
    g.DrawString(text, parent.Font, Brushes.Red, PointF(sx, sy))

type PiHandle() =
  inherit LWControl()

  override this.OnPaint e =
    let g = e.Graphics

    let r = RectangleF(this.Position, this.Size) |> RectF2Rect
    g.DrawEllipse(Pens.Red, r)
  
  override this.HitTest p =
    let pc = this.Position
    let sz = this.Size
    let r = sz.Width / 2.f
    let sqr x = x * x

    let cx, cy = pc.X + r, pc.Y + r
    sqr(p.X - cx) + sqr(p.Y - cy) <= sqr(r)

let up = PIButton(Position=PointF(20.f, 0.f),Size=SizeF(30.f, 30.f),Text="W")
let left = PIButton(Position=PointF(0.f, 30.f),Size=SizeF(30.f, 30.f),Text="A",CoordinateType=View)

let point = PiHandle(Position=PointF(50.f, 50.f),Size=SizeF(20.f, 20.f), CoordinateType=World)

up.MouseDown.Add(fun _ -> printfn "W clicked")
left.MouseDown.Add(fun _ -> printfn "A clicked")
point.MouseDown.Add(fun _ -> printfn "point clicked")

let c = new LWContainer(Dock=DockStyle.Fill)
up.Parent <- c
left.Parent <- c
point.Parent <- c
c.LWControls.Add(up)
c.LWControls.Add(left)
c.LWControls.Add(point)
let f = new Form()
f.Controls.Add(c)
f.Show()
