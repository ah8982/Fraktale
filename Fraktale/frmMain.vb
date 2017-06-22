Public Class frmMain

    ' ######################################## PROGRAMM "HOPALONG" ZUM ZEICHNEN VON ORBIT FRAKTALEN 
    ' ######################################## NACH DEM BARRY-MARTIN-ALGORITHMUS 

    ' ######################################## Variablendeklaration ####################################

    Dim schleifenzahl As Double ' Anzahl der Iterationen 
    Dim a As Double             ' Hopalong-Parameter 
    Dim b As Double             ' Hopalong-Parameter 
    Dim c As Double             ' Hopalong-Parameter 
    Dim x As Double             ' Hopalong-Parameter 
    Dim y As Double             ' Hopalong-Parameter 
    Dim i As Double             ' Iterationsvariable 
    Dim yy As Double            ' Hopalong-Parameter 
    Dim xx As Double            ' Hopalong-Parameter
    Dim yyy As Integer          ' Äußere Schleifenvariable zum Zeichnen 
    Dim xxx As Integer          ' Innere Schleifenvariable zum Zeichnen
    Dim buffer As Double        ' Speichervariable für Zeichenmode 2
    Dim flag As Byte            ' Schaltvariable für Zeichenmode 2
    Dim teilfaktor As Double    ' Größe der Abbildung 
    Dim counter As Integer      ' Farb-Berechnung 
    Dim jumpwert As Double      ' Farb-Berechnung
    Dim stopRender As Boolean = False

    ' ################################## Parameter #####################################################
    Dim breite As Integer = 600       ' Bildbreite (Darf geändert werden)
    Dim höhe As Integer = 400         ' Bildhöhe (Darf geändert werden)
    Dim innenschleife As Integer    ' Maximaler Iterationswert
    Dim stap As Double              ' Über Textbox 3 änderbar: Schrittweite bzw. Auflösung
    Dim schwelle As Double          ' Nur relevant für grafikmodus 2 
    Dim farbfaktor As Integer       ' Kann geändert werden
    Dim farbwert As Integer         ' Farb-Umrechnung

    Private Sub frmMain_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Application.DoEvents()

        ' Textboxen werden beim Start automatisch mit vorgegebenen Standardparametern gefüllt 
        Me.txtLoops.Text = "100000"
        Me.txtParameterA.Text = "98"
        Me.txtParameterB.Text = "0.44"
        Me.txtParameterC.Text = "32.234"
        Me.txtTeilfaktor.Text = "100"
        Me.txtFarbsprungschwelle.Text = 10

        With cboMode.Items
            .Clear()
            .Add("Mode 1")
            .Add("Mode 2")
        End With
        cboMode.Text = "Mode 1"

        Me.ProgressBar1.Style = ProgressBarStyle.Marquee
    End Sub

    Private Sub ModeOne()
        Application.DoEvents()
        Me.ProgressBar1.Style = ProgressBarStyle.Continuous
        ReplaceComma()

        If CheckCommaFarbsprungschwelle() = False Then
            MsgBox("Fehler beim Parameter Farbsprungschwelle.", vbOKOnly, "Fehler")
            Me.txtFarbsprungschwelle.Focus()
            Exit Sub
        End If

        If CheckCommaLoops() = False Then
            MsgBox("Fehler beim Parameter Loops.", vbOKOnly, "Fehler")
            Me.txtLoops.Focus()
            Exit Sub
        End If

        If CheckCommaParameterA() = False Then
            MsgBox("Fehler beim Parameter A", vbOKOnly, "Fehler")
            Me.txtParameterA.Focus()
            Exit Sub
        End If

        If CheckCommaParameterB() = False Then
            MsgBox("Fehler beim Parameter B", vbOKOnly, "Fehler")
            Me.txtParameterB.Focus()
            Exit Sub
        End If

        If CheckCommaParameterC() = False Then
            MsgBox("Fehler beim Parameter C", vbOKOnly, "Fehler")
            Me.txtParameterC.Focus()
            Exit Sub
        End If

        If CheckCommaTeilfaktor() = False Then
            MsgBox("Fehler beim Parameter Teilfaktor", vbOKOnly, "Fehler")
            Me.txtTeilfaktor.Focus()
            Exit Sub
        End If

        'Reset verschiedener Hopalong-Parameter 
        Me.xx = 0
        Me.yy = 0
        Me.x = 0
        Me.y = 0
        Me.counter = 0

        Me.PictureBox1.CreateGraphics.Clear(Color.Black)   'Bildschirm löschen

        'Den Variablen werden die in den TextBoxen gespeicherten Werte zugeordnet. 

        'Die Werte in den TextBoxen können vom Anwender vor dem Start einer neuen Grafik geändert werden 
        Me.a = CInt(Me.txtParameterA.Text)
        Me.b = CInt(Me.txtParameterB.Text)
        Me.c = CInt(Me.txtParameterC.Text)
        Me.schleifenzahl = Me.txtLoops.Text
        Me.jumpwert = Me.txtFarbsprungschwelle.Text
        Me.teilfaktor = Me.txtTeilfaktor.Text

        With Me.ProgressBar1
            .Minimum = 0
            .Maximum = schleifenzahl
            .Value = 0
        End With

        ' Schleife zum Zeichnen der Grafik
        For Me.i = 1 To schleifenzahl
            '(Schleifenzahl kann bis zu mehreren Millionen betragen)

            ProgressBar1.Value = i              ' Fortschrittsbalken aktualisieren
            Application.DoEvents()

            If stopRender = True Then
                stopRender = False
                Me.ProgressBar1.Style = ProgressBarStyle.Marquee
                Exit Sub
            End If

            If Me.counter > 255 Then Me.counter = 1 'Die Variable „counter“ sorgt für die Änderung der Farben beim Zeichnen 

            ' Es folgt der leider etwas kryptische VB-Befehl zum Zeichnen der Pixel an den berechneten Stellen: 
            ' In VB gibt es keinen Befehl zum Zeichen von Pixeln. Am besten eignet sich der Befehl zum Zeichnen von
            ' Rechtecken mit der Kantenlänge 1 (Fill Rectangle)
            ' Die Konstante mit dem Wert 300 bestimmt, wieweit die Grafik vom Nullpunkt (links oben) 
            ' entfernt ist und kann individuell angepasst werden. 

            Me.PictureBox1.CreateGraphics.FillRectangle(New SolidBrush(System.Drawing.Color.FromArgb(255 - Me.counter, _
                                                        Me.counter, 127 + Me.counter / 2)), New Rectangle(10 * x / Me.teilfaktor + 300, 10 * y / Me.teilfaktor + 300, 1, 1))

            'Die Hopalong-Formel: 
            Me.xx = Me.y - Math.Sign(Me.x) * Math.Sqrt(Math.Abs(Me.b * Me.x - Me.c))

            'Ein kleiner "Trick" zum sprunghaften Ändern der Farben
            If Math.Abs(Me.x - Me.xx) < Me.jumpwert Then
                Me.counter += 1
            End If

            'Weiterer Teil der Hopalong-Formel  yy = a - x 
            'Dies sind Alternativen zur obigen Zeile, die ebenfalls interessante Ergebnisse liefern: 
            Me.yy = Me.b - Me.x
            Me.yy = Me.a + Me.b - Me.x
            Me.yy = Me.a - Me.b - Me.x
            Me.yy = Me.b * Me.b - Me.x
            Me.yy = Math.Sqrt(Me.a * Me.b) - Me.x

            'Weiterer Teil der Hopalong-Formel 
            Me.x = Me.xx
            Me.y = Me.yy
        Next
    End Sub

    Private Sub ModeTwo()
        Application.DoEvents()
        Me.ProgressBar1.Style = ProgressBarStyle.Continuous
        ReplaceComma()

        If CheckCommaFarbsprungschwelle() = False Then
            MsgBox("Fehler beim Parameter Farbsprungschwelle.", vbOKOnly, "Fehler")
            Me.txtFarbsprungschwelle.Focus()
            Exit Sub
        End If

        If CheckCommaLoops() = False Then
            MsgBox("Fehler beim Parameter Loops.", vbOKOnly, "Fehler")
            Me.txtLoops.Focus()
            Exit Sub
        End If

        If CheckCommaParameterA() = False Then
            MsgBox("Fehler beim Parameter A", vbOKOnly, "Fehler")
            Me.txtParameterA.Focus()
            Exit Sub
        End If

        If CheckCommaParameterB() = False Then
            MsgBox("Fehler beim Parameter B", vbOKOnly, "Fehler")
            Me.txtParameterB.Focus()
            Exit Sub
        End If

        If CheckCommaParameterC() = False Then
            MsgBox("Fehler beim Parameter C", vbOKOnly, "Fehler")
            Me.txtParameterC.Focus()
            Exit Sub
        End If

        If CheckCommaTeilfaktor() = False Then
            MsgBox("Fehler beim Parameter Teilfaktor", vbOKOnly, "Fehler")
            Me.txtTeilfaktor.Focus()
            Exit Sub
        End If

        With PictureBox1
            Me.breite = CInt(.Width)
            Me.höhe = CInt(.Height)
        End With

        'Abfrage der in den TextBoxen gespeicherten, vorgegebenen Variablen
        Me.a = 0                                           ' Über Textbox 1 änderbar / Startkoordinate X
        Me.stap = txtParameterB.Text                       ' Über TextBox 3 änderbar / Auflösung
        Me.schwelle = txtParameterC.Text                   ' Über TextBox 4 änderbar / Schwelle für mode 2
        Me.innenschleife = txtLoops.Text                   ' Über TextBox 5 änderbar / Anzahl der Iterationen
        Me.farbfaktor = txtFarbsprungschwelle.Text         ' Über TextBox 6 änderbar / Verwandlung der Rechenergebnisse in Farben

        If Me.stap > 0.0004 Then
            Me.stap = CInt(Me.stap) / 10000
        End If

        Me.innenschleife = CInt(Me.innenschleife) / 100
        Me.c = 32.234

        With ProgressBar1
            .Minimum = 0
            .Maximum = breite
            .Value = 0
        End With

        'Bildschirm löschen:
        PictureBox1.CreateGraphics.Clear(Color.Black)

        For Me.yyy = 1 To Me.breite                ' Erste, äußere Verschachtelungsebene: Bildbreite: x-Koordinate

            'Me.Text = "a:" & Me.a.ToString & " b:" & Me.b.ToString & _
            '          " xxx:" & Me.xxx.ToString & " yyy:" & Me.yyy.ToString & " stap:" & Me.stap.ToString

            ProgressBar1.Value = Me.yyy            ' Fortschrittsbalken aktualisieren

            Me.a = Me.a + Me.stap                  ' Inkrementierung des Parameters a um den Wert stap (z.B. 0,004)
            Me.b = 1                               ' Neue "Zeile": b muss auf Anfangswert zurückspringen und wird neu incrementiert

            For Me.xxx = 1 To Me.höhe              ' Zweite Verschachtelungsebene: Bildhöhe: ’ Y-Koordinate zum Zeichnen
                Me.b = Me.b + Me.stap              ' Inkrementierung des Parameters b um den Wert stap

                Me.buffer = 0                      ' Farbwertberechnung: Buffer-Reset
                Me.xx = 0                          ' Hoplaong-Parameter Reset
                Me.yy = 0                          ' Hoplaong-Parameter Reset
                Me.x = 0                           ' Hoplaong-Parameter Reset
                Me.y = 0                           ' Hoplaong-Parameter Reset
                Me.counter = 0

                For Me.i = 1 To Me.innenschleife   'Dritte, innere Verschachtelungsebene 

                    If stopRender = True Then
                        stopRender = False
                        Me.ProgressBar1.Style = ProgressBarStyle.Marquee
                        Exit Sub
                    End If

                    ' Hoplaong-Schleife - Kann von ca. 100 bis unbegrenzt variiert werden 

                    ' Hopalong-Gleichung: 
                    Me.xx = Me.y - Math.Sign(Me.x) * Math.Sqrt(Math.Abs(Me.b * Me.x - Me.c))
                    Me.yy = Me.a - Me.x

                    ' Berechnung der Farbe des zu zeichnenden Pixels 
                    ' Zwei Methoden zur Farbberechnung: Graphmode 0 und  1. Der Modus kann von der 
                    ' Benutzeroberfläche aus gewählt werden. 

                    'Speichern des höchsten Wertes in „buffer“ 
                    If Me.x > Me.buffer Then Me.buffer = Math.Abs(Me.x)

                    ' Teil der Hopalong-Gleichung:
                    Me.x = Me.xx
                    Me.y = Me.yy

                    Application.DoEvents()
                Next Me.i      'Ende der inneren Schleife zur Berechnung eines einzigen Punktes

                ' Umrechnung in Farbwert. Variable "farbfaktor" kann in TextBox 6 geändert werden 
                Me.farbwert = CInt(farbfaktor * buffer)

                If Me.farbwert > 511 Then Me.farbwert = 511
                If Me.farbwert > 255 And Me.farbwert < 512 Then Me.farbwert = farbwert - 256

                ' Es folgt das Zeichnen des betreffenden Farbpixels an den Koordinaten yyy und xxx.
                ' Der Einsatz der Variablen "farbwert" als rgb-Werte ist nur ein Beispiel.
                ' Die besten Ergebnisse lassen sich mit Paletten-Arrays erzielen, in denen zu jedem Wert
                ' der Variablen "farbwert" die jeweiligen RGB-Werte gezielt nach Gesichtspunkten der
                ' optimalen Wahrnehmung bzw. der Ästhetik eingetragen sind.

                PictureBox1.CreateGraphics.FillRectangle(New SolidBrush _
                            (System.Drawing.Color.FromArgb(Me.farbwert, 125 + Me.farbwert / 2, 255 - Me.farbwert)), _
                            New Rectangle(Me.yyy, Me.xxx, 1, 1))
            Next Me.xxx
        Next Me.yyy
    End Sub

    Private Sub btnStart_Click(sender As System.Object, e As System.EventArgs) Handles btnStart.Click
        Application.DoEvents()

        With cboMode
            Select Case .SelectedItem.ToString
                Case Is = "Mode 1"
                    ModeOne()
                Case Is = "Mode 2"
                    ModeTwo()
            End Select
        End With
    End Sub

    Private Sub btnStop_Click(sender As System.Object, e As System.EventArgs) Handles btnStop.Click
        Me.stopRender = True
    End Sub

    Private Sub cboMode_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cboMode.SelectedIndexChanged
        With cboMode
            Select Case .SelectedItem.ToString
                Case Is = "Mode 1"
                    Me.txtFarbsprungschwelle.Enabled = True
                    Me.txtLoops.Enabled = True
                    Me.txtParameterA.Enabled = True
                    Me.txtParameterB.Enabled = True
                    Me.txtParameterC.Enabled = True
                    Me.txtTeilfaktor.Enabled = True
                Case Is = "Mode 2"
                    Me.txtFarbsprungschwelle.Enabled = True
                    Me.txtLoops.Enabled = True
                    Me.txtParameterA.Enabled = False
                    Me.txtParameterB.Enabled = True
                    Me.txtParameterC.Enabled = True
                    Me.txtTeilfaktor.Enabled = True
            End Select
        End With
    End Sub

    Private Sub txtReset_Click(sender As System.Object, e As System.EventArgs) Handles btnReset.Click
        Me.txtLoops.Text = 100000
        Me.txtParameterA.Text = 98
        Me.txtParameterB.Text = "0.44"
        Me.txtParameterC.Text = "32.234"
        Me.txtTeilfaktor.Text = 100
        Me.txtFarbsprungschwelle.Text = 10

        Me.txtFarbsprungschwelle.Enabled = True
        Me.txtLoops.Enabled = True
        Me.txtParameterA.Enabled = True
        Me.txtParameterB.Enabled = True
        Me.txtParameterC.Enabled = True
        Me.txtTeilfaktor.Enabled = True

        With cboMode.Items
            .Clear()
            .Add("Mode 1")
            .Add("Mode 2")
        End With
        cboMode.Text = "Mode 1"

        Me.ProgressBar1.Style = ProgressBarStyle.Marquee
    End Sub

    Private Sub ReplaceComma()
        Me.txtLoops.Text = Replace(Me.txtLoops.Text, ",", ".")
        Me.txtParameterA.Text = Replace(Me.txtParameterA.Text, ",", ".")
        Me.txtParameterB.Text = Replace(Me.txtParameterB.Text, ",", ".")
        Me.txtParameterC.Text = Replace(Me.txtParameterC.Text, ",", ".")
        Me.txtTeilfaktor.Text = Replace(Me.txtTeilfaktor.Text, ",", ".")
        Me.txtFarbsprungschwelle.Text = Replace(Me.txtFarbsprungschwelle.Text, ",", ".")
    End Sub

    Private Function CheckCommaLoops()
        Dim IsOK As Boolean = True
        Dim count As Integer = 0
        Dim x As Integer = 0

        For x = 1 To Len(Me.txtLoops.Text)
            If Mid(Me.txtLoops.Text, x, 1) = "." Or Mid(Me.txtLoops.Text, x, 1) = "." Then
                count += 1
            End If
        Next x

        If count > 1 Then
            IsOK = False
        End If

        Return IsOK
    End Function

    Private Function CheckCommaParameterA()
        Dim IsOK As Boolean = True
        Dim count As Integer = 0
        Dim x As Integer = 0

        For x = 1 To Len(Me.txtParameterA.Text)
            If Mid(Me.txtParameterA.Text, x, 1) = "." Or Mid(Me.txtParameterA.Text, x, 1) = "." Then
                count += 1
            End If
        Next x

        If count > 1 Then
            IsOK = False
        End If

        Return IsOK
    End Function

    Private Function CheckCommaParameterB()
        Dim IsOK As Boolean = True
        Dim count As Integer = 0
        Dim x As Integer = 0

        For x = 1 To Len(Me.txtParameterB.Text)
            If Mid(Me.txtParameterB.Text, x, 1) = "." Or Mid(Me.txtParameterB.Text, x, 1) = "." Then
                count += 1
            End If
        Next x

        If count > 1 Then
            IsOK = False
        End If

        Return IsOK
    End Function

    Private Function CheckCommaParameterC()
        Dim IsOK As Boolean = True
        Dim count As Integer = 0
        Dim x As Integer = 0

        For x = 1 To Len(Me.txtParameterC.Text)
            If Mid(Me.txtParameterC.Text, x, 1) = "." Or Mid(Me.txtParameterC.Text, x, 1) = "." Then
                count += 1
            End If
        Next x

        If count > 1 Then
            IsOK = False
        End If

        Return IsOK
    End Function

    Private Function CheckCommaTeilfaktor()
        Dim IsOK As Boolean = True
        Dim count As Integer = 0
        Dim x As Integer = 0

        For x = 1 To Len(Me.txtTeilfaktor.Text)
            If Mid(Me.txtTeilfaktor.Text, x, 1) = "." Or Mid(Me.txtTeilfaktor.Text, x, 1) = "." Then
                count += 1
            End If
        Next x

        If count > 1 Then
            IsOK = False
        End If

        Return IsOK
    End Function

    Private Function CheckCommaFarbsprungschwelle()
        Dim IsOK As Boolean = True
        Dim count As Integer = 0
        Dim x As Integer = 0

        For x = 1 To Len(Me.txtFarbsprungschwelle.Text)
            If Mid(Me.txtFarbsprungschwelle.Text, x, 1) = "." Or Mid(Me.txtFarbsprungschwelle.Text, x, 1) = "." Then
                count += 1
            End If
        Next x

        If count > 1 Then
            IsOK = False
        End If

        Return IsOK
    End Function

    Private Sub frmMain_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'Den Arbeistsspeicher aufräumen.
        With Me
            .PictureBox1.CreateGraphics.Clear(Color.Black)
            .PictureBox1 = Nothing

            .schleifenzahl = 0
            .schleifenzahl = Nothing
            .a = 0
            .a = Nothing
            .b = 0
            .b = Nothing
            .c = 0
            .c = Nothing
            .x = 0
            .x = Nothing
            .y = 0
            .y = Nothing
            .i = 0
            .i = Nothing
            .xx = 0
            .xx = Nothing
            .yy = 0
            .yy = Nothing
            .xxx = 0
            .xxx = Nothing
            .yyy = 0
            .yyy = Nothing
            .buffer = 0
            .buffer = Nothing
            .flag = Nothing
            .teilfaktor = 0
            .teilfaktor = Nothing
            .counter = 0
            .counter = Nothing
            .jumpwert = 0
            .jumpwert = Nothing
            .stopRender = Nothing
            .breite = 0
            .breite = Nothing
            .höhe = 0
            .höhe = Nothing
            .innenschleife = 0
            .innenschleife = Nothing
            .stap = 0
            .stap = Nothing
            .schwelle = 0
            .schwelle = Nothing
            .farbfaktor = 0
            .farbfaktor = Nothing
            .farbwert = 0
            .farbwert = Nothing

            .txtLoops = Nothing
            .txtParameterA = Nothing
            .txtParameterB = Nothing
            .txtParameterC = Nothing
            .txtTeilfaktor = Nothing
            .txtFarbsprungschwelle = Nothing

            .btnStart = Nothing
            .btnStop = Nothing
            .btnReset = Nothing

            .ProgressBar1 = Nothing
        End With
    End Sub
End Class