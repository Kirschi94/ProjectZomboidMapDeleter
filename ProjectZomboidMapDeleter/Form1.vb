Imports System.ComponentModel
Imports System.Text.RegularExpressions

Public Class Form1
#Region "Global variables"
    Dim TB1 As String = Nothing
    Dim TB2 As String = Nothing
    Dim TB3 As String = Nothing
    Dim TB4 As String = Nothing
    Dim TB5 As String = Nothing
    Dim TB6 As String = Nothing
    Dim CB1 As Boolean = Nothing
    Dim CB2 As Boolean = Nothing

    Dim DeletedFiles As Integer = Nothing
    Dim Errors As Integer = Nothing
    Dim Errorlist As String = Nothing

    Dim Steps As Integer = 0
    Dim LastPercentage As Integer = 100
#End Region

#Region "Functions"
    Private Function Do_Coords(TBText As String)
        Dim TempString As String() = TBText.Split("x")
        For Each element In TempString
            element.Replace("x", "")
        Next
        TempString(0) = TempString(0).Remove(TempString(0).Length - 1, 1)
        TempString(1) = TempString(1).Remove(TempString(1).Length - 1, 1)
        Return TempString
    End Function
#End Region

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
#Region "Exclusion"
        Dim Counter As Integer
        Dim Exclude As New List(Of String)
        If CB1 Then
            Dim FirstEx As String() = Do_Coords(TB4)
            Dim SecondEx As String() = Do_Coords(TB6)

            Dim Partm1a As Integer
            Dim Partm1b As Integer

            If FirstEx(0) < SecondEx(0) Then Partm1a = FirstEx(0) : Partm1b = SecondEx(0) _
            Else Partm1a = SecondEx(0) : Partm1b = FirstEx(0)

            Dim FirstExL As New List(Of String)
            While Partm1a <= Partm1b
                FirstExL.Add(Partm1a.ToString())
                Partm1a += 1
            End While
            If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub

            Dim Part0a As Integer
            Dim Part0b As Integer
            If FirstEx(1) < SecondEx(1) Then Part0a = FirstEx(1) : Part0b = SecondEx(1) _
            Else Part0a = SecondEx(1) : Part0b = FirstEx(1)

            Dim SecondExL As New List(Of String)
            While Part0a <= Part0b
                SecondExL.Add(Part0a.ToString())
                Part0a += 1
            End While
            If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub

            Dim AllCombA As Integer = FirstExL.Count * SecondExL.Count
            Counter = 0
            Dim StarttimeA As DateTime = DateTime.Now
            For Each ElementA In FirstExL
                For Each ElementB In SecondExL
                    Exclude.Add($"map_{ElementA}_{ElementB}.bin")
                    Counter += 1
                    Dim TimeSpent As New TimeSpan(Now.Ticks - StarttimeA.Ticks)
                    Dim SecondsRemaining As Integer = TimeSpent.TotalSeconds / (100 / AllCombA * Counter) * (100 - (100 / AllCombA * Counter))
                    BackgroundWorker1.ReportProgress(100 / AllCombA * Counter, SecondsRemaining)
                Next
                If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub
            Next
        End If
#End Region
#Region "Construction"
        Dim DeletionList As New List(Of String)
        Dim FirstCords As String() = Do_Coords(TB1)
        Dim SecondCords As String() = Do_Coords(TB2)

        Dim Part1a As Integer
        Dim Part1b As Integer
        If FirstCords(0) < SecondCords(0) Then Part1a = FirstCords(0) : Part1b = SecondCords(0) _
            Else Part1a = SecondCords(0) : Part1b = FirstCords(0)

        Dim FirstPart As New List(Of String)
        While Part1a <= Part1b
            FirstPart.Add(Part1a.ToString())
            Part1a += 1
        End While
        If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub

        Dim Part2a As Integer
        Dim Part2b As Integer
        If FirstCords(1) < SecondCords(1) Then Part2a = FirstCords(1) : Part2b = SecondCords(1) _
        Else Part2a = SecondCords(1) : Part2b = FirstCords(1)

        Dim SecondPart As New List(Of String)
        While Part2a <= Part2b
            SecondPart.Add(Part2a.ToString())
            Part2a += 1
        End While
        If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub


        Dim AllCombB As Integer = FirstPart.Count * SecondPart.Count
        Counter = 0
        Dim StarttimeB As DateTime = DateTime.Now
        For Each ElementA In FirstPart
            For Each ElementB In SecondPart
                Dim TempString = $"map_{ElementA}_{ElementB}.bin"
                If Not Exclude.Contains(TempString) Then DeletionList.Add(TempString)
                Counter += 1
                Dim TimeSpent As New TimeSpan(DateTime.Now.Ticks - StarttimeB.Ticks)
                Dim SecondsRemaining As Integer = TimeSpent.TotalSeconds / (100 / AllCombB * Counter) * (100 - (100 / AllCombB * Counter))
                BackgroundWorker1.ReportProgress(100 / AllCombB * Counter, SecondsRemaining)
            Next
            If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub
        Next
#End Region
#Region "Deletion"
        Dim Mappath As String = TB3
        Dim Backuppath As String = TB5
        If Not Mappath.EndsWith("\") Then Mappath &= "\"
        If Not Backuppath.EndsWith("\") Then Backuppath &= "\"

        Dim DeletedFiles_i As Integer
        Dim Errors_i As Integer = 0
        Dim ErrorList_i As String = ""

        Counter = 0
        Dim StarttimeC As DateTime = DateTime.Now
        For Each Element In DeletionList
            Try
                If CB2 Then IO.File.Copy(Mappath & Element, Backuppath & Element)
                IO.File.Delete(Mappath & Element)
                DeletedFiles_i += 1
            Catch ex As Exception
                ErrorList_i &= ex.Message & vbCrLf
                Errors_i += 1
            End Try
            Counter += 1
            Dim TimeSpent As New TimeSpan(DateTime.Now.Ticks - StarttimeC.Ticks)
            Dim SecondsRemaining As Integer = TimeSpent.TotalSeconds / (100 / DeletionList.Count * Counter) * (100 - (100 / DeletionList.Count * Counter))
            BackgroundWorker1.ReportProgress(100 / DeletionList.Count * Counter, SecondsRemaining)
            If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub
        Next

        DeletedFiles = DeletedFiles_i
        Errors = Errors_i
        Errorlist = ErrorList_i
#End Region
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
#Region "RegEx"
        If Not Regex.IsMatch(TextBox1.Text, "^[\d]{2,7}x[\d]{2,7}$") Then MessageBox.Show("Please fill in every field (correctly).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) : Exit Sub
        If Not Regex.IsMatch(TextBox2.Text, "^[\d]{2,7}x[\d]{2,7}$") Then MessageBox.Show("Please fill in every field (correctly).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) : Exit Sub
        If CheckBox1.Checked Then
            If Not Regex.IsMatch(TextBox4.Text, "^[\d]{2,7}x[\d]{2,7}$") Then MessageBox.Show("Please fill in every field (correctly).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) : Exit Sub
            If Not Regex.IsMatch(TextBox6.Text, "^[\d]{2,7}x[\d]{2,7}$") Then MessageBox.Show("Please fill in every field (correctly).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) : Exit Sub
        End If
        If Not Regex.IsMatch(TextBox3.Text, "(?>[A-Za-z]+:|\\)(?:\\[^\\?*]*)+") Then MessageBox.Show("Please fill in every field (correctly).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) : Exit Sub
        If CheckBox2.Checked Then If Not Regex.IsMatch(TextBox5.Text, "(?>[A-Za-z]+:|\\)(?:\\[^\\?*]*)+") Then MessageBox.Show("Please fill in every field (correctly).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) : Exit Sub
#End Region
#Region "Check Folder"
        Dim Mappath As String = TextBox3.Text
        Dim Backuppath As String = TextBox5.Text
        If Not Mappath.EndsWith("\") Then Mappath &= "\"
        If Not Backuppath.EndsWith("\") Then Backuppath &= "\"

        If Not IO.Directory.Exists(Mappath) Then MessageBox.Show("Mapfile-Path does not exist.", "Error") : Exit Sub
        If CheckBox2.Checked And IO.Directory.Exists(Backuppath) And IO.Directory.GetFiles(Backuppath).Length > 0 Then _
            If MessageBox.Show($"The folder you chose for backing up the mapfiles seems to be in use.{vbCrLf}Do you still want to use it?", "Warning",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Exit Sub
        If CheckBox2.Checked And Not IO.Directory.Exists(Backuppath) Then
            Try
                IO.Directory.CreateDirectory(Backuppath)
            Catch ex As Exception
                MessageBox.Show($"Backups-Path could not be created:{vbCrLf}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) : Exit Sub
            End Try
        End If

        Dim Foldercheck As Integer = 0
        For Each file In IO.Directory.GetFiles(Mappath)
            If Regex.IsMatch(file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)), "^chunkdata_[\d]{1,3}_[\d]{1,3}\.bin$") Then Foldercheck += 1 : Exit For
        Next
        For Each file In IO.Directory.GetFiles(Mappath)
            If file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)).Contains("rosion.in") Then Foldercheck += 1 : Exit For
        Next
        For Each file In IO.Directory.GetFiles(Mappath)
            If Regex.IsMatch(file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)), "^map_[\d]{2,6}_[\d]{2,6}\.bin$") Then Foldercheck += 1 : Exit For
        Next
        For Each file In IO.Directory.GetFiles(Mappath)
            If Regex.IsMatch(file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)), "^zpop_[\d]{1,3}_[\d]{1,3}\.bin$") Then Foldercheck += 1 : Exit For
        Next
        For Each file In IO.Directory.GetFiles(Mappath)
            If file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)).Contains("orldDictionary.bi") Then Foldercheck += 1 : Exit For
        Next
        For Each file In IO.Directory.GetFiles(Mappath)
            If file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)).Contains("ehicles.d") Then Foldercheck += 1 : Exit For
        Next
        For Each file In IO.Directory.GetFiles(Mappath)
            If file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)).Contains("layers.d") Then Foldercheck += 1 : Exit For
        Next
        For Each file In IO.Directory.GetFiles(Mappath)
            If file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)).Contains("ap_zone.bi") Then Foldercheck += 1 : Exit For
        Next
        For Each file In IO.Directory.GetFiles(Mappath)
            If file.Substring(file.LastIndexOf("\") + 1, file.Length - (file.LastIndexOf("\") + 1)).Contains("ap_meta.bi") Then Foldercheck += 1 : Exit For
        Next
        If Foldercheck <= 6 Then If MessageBox.Show($"It seems you have selected the wrong folder for the mapfiles.{vbCrLf}Do you still want to continue?", "Warning",
                                                 MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then Exit Sub
#End Region
#Region "Button Actions"
        If Button1.Text = "Go" Then
            TB1 = TextBox1.Text
            TB2 = TextBox2.Text
            TB3 = TextBox3.Text
            TB4 = TextBox4.Text
            TB5 = TextBox5.Text
            TB6 = TextBox6.Text
            CB1 = CheckBox1.Checked
            CB2 = CheckBox2.Checked
            BackgroundWorker1.RunWorkerAsync()
            Button1.Text = "Abort"
        Else
            Button1.Enabled = False
            BackgroundWorker1.CancelAsync()
        End If
#End Region
    End Sub
#Region "Windows Forms Actions"
    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        MessageBox.Show($"{DeletedFiles:#,##0} files have been deleted, {Errors:#,##0} errors have occurred.", "Process finished", MessageBoxButtons.OK)
        If Errors > 0 Then
            Dim ErrString As String = $"While deleting map-files between the coordinates {TB1} and {TB2}, the following {Errors:#,##0} errors occurred:{vbCrLf}{vbCrLf}{vbCrLf}{vbCrLf}"
            If CB2 Then
                Dim Backuppath As String = TB5
                If Not Backuppath.EndsWith("\") Then Backuppath &= "\"
                Try
                    Dim Filepath As String = Backuppath & $"PZMD Errorlog {Now:yy-MM-dd, HH-mm-ss}h.log"
                    IO.File.WriteAllText(Filepath, ErrString & Errorlist)
                    MessageBox.Show($"An errorlog has been created at ""{Filepath}"".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("There was an error trying to export the errorlog.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Else
                Try
                    Dim Filepath As String = Environment.SpecialFolder.Desktop & $"PZMD Errorlog {Now:yy-MM-dd, HH-mm-ss}h.log"
                    IO.File.WriteAllText(Filepath, ErrString & Errorlist)
                    MessageBox.Show($"An errorlog has been created on your desktop with path ""{Filepath}"".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("There was an error trying to export the errorlog.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End If
        If Not ProgressBar1.Value = 100 Then ProgressBar1.Value = 100
        Steps = 0
        ToolStripLabel1.Text = $"Step {Steps}"
        ToolStripLabel2.Text = "Est. time: 00:00"
        ToolTip1.SetToolTip(ProgressBar1, Nothing)
        LastPercentage = 100
        Button1.Enabled = True
        Button1.Text = "Go"
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        If LastPercentage > e.ProgressPercentage Then Steps += 1 : ToolStripLabel1.Text = $"Step {Steps}"
        LastPercentage = e.ProgressPercentage
        ProgressBar1.Value = e.ProgressPercentage
        ToolTip1.SetToolTip(ProgressBar1, $"{e.ProgressPercentage:N}%")
        Dim TempSpan As New TimeSpan(0, 0, Convert.ToInt32(e.UserState))
        If TempSpan.Hours > 0 Then ToolStripLabel2.Text = $"Est. time: {New DateTime(TempSpan.Ticks):HH:mm:ss}" _
            Else ToolStripLabel2.Text = $"Est. time: {New DateTime(TempSpan.Ticks):mm:ss}"
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        TextBox4.Enabled = CheckBox1.Checked : TextBox6.Enabled = CheckBox1.Checked
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        TextBox5.Enabled = CheckBox2.Checked
    End Sub
#End Region
#Region "Form opening and closing"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Load_And_Apply_ini()
        CheckBox1_CheckedChanged(Nothing, Nothing)
        CheckBox2_CheckedChanged(Nothing, Nothing)
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If BackgroundWorker1.IsBusy Then e.Cancel = True : MessageBox.Show("The program cannot be closed while it is busy.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information) _
        Else Save_ini()
    End Sub
#End Region
#Region ".ini handling"
    Private Sub Save_ini()
Resave:
        Try
            Dim iniString As String = ""
            iniString &= $"TB1:""{TextBox1.Text}""{vbCrLf}"
            iniString &= $"TB2:""{TextBox2.Text}""{vbCrLf}"
            iniString &= $"TB3:""{TextBox3.Text}""{vbCrLf}"
            iniString &= $"TB4:""{TextBox4.Text}""{vbCrLf}"
            iniString &= $"TB5:""{TextBox5.Text}""{vbCrLf}"
            iniString &= $"TB6:""{TextBox6.Text}""{vbCrLf}"
            iniString &= $"CB1:{CheckBox1.Checked}{vbCrLf}"
            iniString &= $"CB2:{CheckBox2.Checked}{vbCrLf}"
            IO.File.WriteAllText(Application.StartupPath & "\settings.ini", iniString)
        Catch ex As Exception
            If MessageBox.Show("Settings file could not be saved. Do you want to try again?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then GoTo Resave _
                Else MessageBox.Show($"Settings file could not be saved due to the following error:{vbCrLf}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub
    Private Sub Load_And_Apply_ini(Optional path As String = Nothing)
Reload:
        Try
            If IsNothing(path) Then path = Application.StartupPath & "\settings.ini"
            If Not IO.File.Exists(path) Then Exit Sub
            Dim iniLines As String() = IO.File.ReadAllLines(path)
            Dim EmptyLineCounter As Integer = 0
            For Each Line In iniLines
                If Not Line.Length = 0 AndAlso Not Line = "" AndAlso Not IsNothing(Line) Then
                    If Line.StartsWith("TB1:") Then TextBox1.Text = Line.Substring(5, Line.Length - (5 + 1)) : Continue For
                    If Line.StartsWith("TB2:") Then TextBox2.Text = Line.Substring(5, Line.Length - (5 + 1)) : Continue For
                    If Line.StartsWith("TB3:") Then TextBox3.Text = Line.Substring(5, Line.Length - (5 + 1)) : Continue For
                    If Line.StartsWith("TB4:") Then TextBox4.Text = Line.Substring(5, Line.Length - (5 + 1)) : Continue For
                    If Line.StartsWith("TB5:") Then TextBox5.Text = Line.Substring(5, Line.Length - (5 + 1)) : Continue For
                    If Line.StartsWith("TB6:") Then TextBox6.Text = Line.Substring(5, Line.Length - (5 + 1)) : Continue For
                    If Line.StartsWith("CB1:") Then CheckBox1.Checked = Line.Substring(4, Line.Length - 4) : Continue For
                    If Line.StartsWith("CB2:") Then CheckBox2.Checked = Line.Substring(4, Line.Length - 4) : Continue For
                Else
                    EmptyLineCounter += 1
                End If
            Next
            If (iniLines.Length - EmptyLineCounter) < 8 Then MessageBox.Show("Settings file could not be read properly. Some saved settings might not have been applied.",
                                                                             "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            If MessageBox.Show("Settings file could not be read. Do you want to try again?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then GoTo Reload _
                Else MessageBox.Show($"Settings file could not be read due to the following error:{vbCrLf}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub
#End Region
End Class
