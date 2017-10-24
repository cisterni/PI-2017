#load "lwc.fsx"

open System.Windows.Forms
open System.Drawing

open Lwc

type UpButton() =
  inherit LWControl()

  override this.OnPaint e =
    let parent = this.Parent
    let g = e.Graphics
    let r = Rectangle(20, 0, 30, 30)
    g.DrawRectangle(Pens.Red, r)
    let sz = g.MeasureString("W", parent.Font)
    g.DrawString("W", parent.Font, Brushes.Red, PointF(20.f + (30.f - sz.Width) / 2.f, (30.f - sz.Height) / 2.f))

let up = new UpButton()

let c = new LWContainer(Dock=DockStyle.Fill)
up.Parent <- c
c.LWControls.Add(up)
let f = new Form()
f.Controls.Add(c)
f.Show()
