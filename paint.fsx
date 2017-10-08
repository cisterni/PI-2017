open System.Windows.Forms
open System.Drawing

let f = new Form(TopMost=true)
f.Show()

f.Paint.Add(fun e ->
  let g = e.Graphics // Contesto grafico
  g.DrawLine(Pens.Black, 0, 0, 100, 100)
)

f.Invalidate() // Richiesta di un evento paint

let g = Graphics.FromHwnd(f.Handle)
g.DrawLine(Pens.Red, 100, 0, 0, 100)

type AnalogClock() =
  inherit UserControl()

  let ticksize = 10.

  override this.OnResize _ =
    this.Invalidate()

  override this.OnPaint e =
    let g = e.Graphics
    let a = this.ClientSize
    let mx, my = double(a.Width / 2), double(a.Height / 2)
    let r2 = min mx my
    let r1 = r2 - ticksize
    let dalpha = System.Math.PI / 6.0
    for i in 0 .. 11 do
      let alpha = double(i) * dalpha
      let x2, y2 = mx + r2 * cos alpha, my + r2 * sin alpha
      let x1, y1 = mx + r1 * cos alpha, my + r1 * sin alpha
      let p1 = PointF(single(x1), single(y1))
      let p2 = PointF(single(x2), single(y2))
      g.DrawLine(Pens.Black, p1, p2)

let ff = new Form(TopMost=true)
let clock = new AnalogClock(Dock=DockStyle.Fill)
//clock.BackColor <- Color.Red
ff.Controls.Add(clock)
ff.Show()
