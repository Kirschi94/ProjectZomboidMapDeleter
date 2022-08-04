Imports System.ComponentModel

Public Class Form1
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
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "" Then MessageBox.Show("Please fill in every field.", "Error") : Exit Sub
        If TextBox2.Text = "" Then MessageBox.Show("Please fill in every field.", "Error") : Exit Sub
        If TextBox3.Text = "" Then MessageBox.Show("Please fill in every field.", "Error") : Exit Sub
        If CheckBox1.Checked And (TextBox4.Text = "" Or TextBox6.Text = "") Then MessageBox.Show("Please fill in every field.", "Error") : Exit Sub
        If CheckBox2.Checked And TextBox5.Text = "" Then MessageBox.Show("Please fill in every field.", "Error") : Exit Sub

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
    End Sub

    Private Function Do_Coords(TBText As String)
        Dim TempString As String() = TBText.Split("x")
        For Each element In TempString
            element.Replace("x", "")
        Next
        TempString(0) = TempString(0).Remove(TempString(0).Length - 1, 1)
        TempString(1) = TempString(1).Remove(TempString(1).Length - 1, 1)
        Return TempString
    End Function

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

        If Not IO.Directory.Exists(TB3) Then MessageBox.Show("Error", "Mapfile-Path does not exist.") : Exit Sub
        If CB2 And Not IO.Directory.Exists(TB5) Then
            Try
                IO.Directory.CreateDirectory(TB5)
            Catch ex As Exception
                MessageBox.Show("Error", "Backups-Path could not be created.")
                Exit Sub
            End Try
        End If

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

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        MessageBox.Show($"{DeletedFiles:#,##0} files have been deleted, {Errors:#,##0} errors have occurred.", "Process finished")
        If Errors > 0 Then
            Dim ErrString As String = $"While deleting map-files between the coordinates {TB1} and {TB2}, the following {Errors:#,##0} errors occurred:{vbCrLf}{vbCrLf}{vbCrLf}{vbCrLf}"
            If CB2 Then
                Dim Backuppath As String = TB5
                If Not Backuppath.EndsWith("\") Then Backuppath &= "\"
                Try
                    Dim Filepath As String = Backuppath & $"{Now:PZMD Errorlog yy-MM-dd, HH-mm-ss.log}"
                    IO.File.WriteAllText(Filepath, ErrString & Errorlist)
                    MessageBox.Show($"An errorlog has been created at ""{Filepath}"".", "Error")
                Catch ex As Exception
                    MessageBox.Show("There was an error trying to export the errorlog.", "Error")
                End Try
            Else
                Try
                    Dim Filepath As String = Environment.SpecialFolder.Desktop & $"{Now:PZMD Errorlog yy-MM-dd, HH-mm-ss.log}"
                    IO.File.WriteAllText(Filepath, ErrString & Errorlist)
                    MessageBox.Show($"An errorlog has been created on your desktop with path ""{Filepath}"".", "Error")
                Catch ex As Exception
                    MessageBox.Show("There was an error trying to export the errorlog.", "Error")
                End Try
            End If
        End If
        'If Not Errorlist = Nothing And Not Errorlist = "" Then MessageBox.Show($"Errorlist:{vbCrLf}{Errorlist}")
        If Not ProgressBar1.Value = 100 Then ProgressBar1.Value = 100
        Steps = 0
        ToolStripLabel1.Text = $"Step {Steps}"
        ToolStripLabel2.Text = "Est. time: 00:00"
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

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        CheckBox1_CheckedChanged(Nothing, Nothing)
        CheckBox2_CheckedChanged(Nothing, Nothing)
    End Sub
End Class
