open System.Windows.Forms
open System.Drawing

type AnalogClock() as this =
  inherit UserControl()

  let ticksize = 10.

  let timer = new Timer()

  let mutable offscreen = new Bitmap(this.Width, this.Height)

  do
    timer.Interval <- 1000
    timer.Tick.Add(fun _ -> this.Invalidate())
    timer.Start()


  override this.OnResize _ =
    this.Invalidate()
    offscreen.Dispose()
    offscreen <- new Bitmap(this.Width, this.Height)
  
  override this.OnPaintBackground _ = ()

  override this.OnPaint e =
    let g = Graphics.FromImage(offscreen)
    use bg = new SolidBrush(this.BackColor)
    g.SmoothingMode <- Drawing2D.SmoothingMode.HighQuality
    g.FillRectangle(bg, this.ClientRectangle)
    let a = this.ClientSize
    let mx, my = single(a.Width / 2), single(a.Height / 2)
    let r = min (a.Width / 2) (a.Height / 2) |> single
    let t = g.Transform
    t.Translate(mx, my)
    t.Scale(1.f, -1.f)
    g.Transform <- t
    let t0 = t.Clone()
    // g.Save()

    let dalpha = 180.0 / 6.0
    let p1 = PointF(0.f, r)
    let p2 = PointF(0.f, r - single(ticksize))
    for i in 0 .. 11 do
      g.DrawLine(Pens.Black, p1, p2)
      t.Rotate(single(dalpha))
      g.Transform <- t
    
    g.Transform <- t0
    let now = System.DateTime.Now
    let h = (single(now.Hour * 60 + now.Minute) / (12.f * 60.f)) * 360.f
    let m = (single(now.Minute) / 60.f ) * 360.f
    let s = single(now.Second) * 6.f
    let lancetta (angle, len, width) =
      use p = new Pen(Color.Black, width)
      let m = t0.Clone()
      m.Rotate(-angle)
      g.Transform <- m
      g.DrawLine(p, 0, -10, 0, int(len) - 10)
      g.Transform <- t0
    let lh = r * 3.f / 4.f
    let lm = r
    let ls = r

    lancetta (h, lh, 3.f)
    lancetta (m, lm, 2.f)
    lancetta (s, ls, 1.f)


    let gg = e.Graphics
    gg.DrawImage(offscreen, 0, 0)
let ff = new Form(TopMost=true)
let clock = new AnalogClock(Dock=DockStyle.Fill)
//clock.BackColor <- Color.Red
ff.Controls.Add(clock)
ff.Show()
