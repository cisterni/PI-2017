open System.Windows.Forms

let f= new Form(TopMost=true)
f.Show()

let wb = new WebBrowser(Dock=DockStyle.Fill)
f.Controls.Add(wb)

wb.Navigate("http://www.unipi.it")

let tb = new TextBox(Dock=DockStyle.Top)
f.Controls.Add(tb)

wb.Navigating.Add(fun e ->
 tb.Text <- e.Url.ToString()
)

tb.KeyDown.Add(fun e ->
  if e.KeyData = Keys.Enter then
    wb.Navigate(tb.Text)
)

// Marquee