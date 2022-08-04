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

            'MessageBox.Show($"FirstEx: {FirstEx(0)} : {FirstEx(1)}{vbCrLf}SecondEx: {SecondEx(0)} : {SecondEx(1)}")
            'MessageBox.Show($"Partm1a: {Partm1a}{vbCrLf}Partm1b: {Partm1b}")

            'MessageBox.Show("Schwannek0")

            Dim FirstExL As New List(Of String)
            Dim StartValA As Integer = Partm1a
            While Partm1a <= Partm1b
                FirstExL.Add(Partm1a.ToString())
                Partm1a += 1
                'BackgroundWorker1.ReportProgress(100 - (100 / (Partm1b - StartValA) * (Partm1b - Partm1a)))
            End While
            If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub
            'MessageBox.Show("Schwannek1")
            Dim Part0a As Integer
            Dim Part0b As Integer
            If FirstEx(1) < SecondEx(1) Then Part0a = FirstEx(1) : Part0b = SecondEx(1) _
            Else Part0a = SecondEx(1) : Part0b = FirstEx(1)

            Dim SecondExL As New List(Of String)
            Dim StartValB As Integer = Part0a
            While Part0a <= Part0b
                SecondExL.Add(Part0a.ToString())
                Part0a += 1
                'BackgroundWorker1.ReportProgress(100 - (100 / (Part0b - StartValB) * (Part0b - Part0a)))
            End While
            If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub
            'MessageBox.Show("Schwannek2")
            Dim AllCombA As Integer = FirstExL.Count * SecondExL.Count
            Counter = 0
            For Each ElementA In FirstExL
                For Each ElementB In SecondExL
                    Exclude.Add($"map_{ElementA}_{ElementB}.bin")
                    Counter += 1
                    BackgroundWorker1.ReportProgress(100 / AllCombA * Counter)
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
        'MessageBox.Show("Schwannek3")
        Dim FirstPart As New List(Of String)
        Dim StartValC As Integer = Part1a
        While Part1a <= Part1b
            FirstPart.Add(Part1a.ToString())
            Part1a += 1
            'BackgroundWorker1.ReportProgress(100 - (100 / (Part1b - StartValC) * (Part1b - Part1a)))
        End While
        If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub
        'MessageBox.Show("Schwannek4")
        Dim Part2a As Integer
        Dim Part2b As Integer
        If FirstCords(1) < SecondCords(1) Then Part2a = FirstCords(1) : Part2b = SecondCords(1) _
        Else Part2a = SecondCords(1) : Part2b = FirstCords(1)

        Dim SecondPart As New List(Of String)
        Dim StartValD As Integer = Part1a
        While Part2a <= Part2b
            SecondPart.Add(Part2a.ToString())
            Part2a += 1
            'BackgroundWorker1.ReportProgress(100 - (100 / (Part2b - StartValD) * (Part2b - Part2a)))
        End While
        If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub


        Dim AllCombB As Integer = FirstPart.Count * SecondPart.Count
        Counter = 0
        For Each ElementA In FirstPart
            For Each ElementB In SecondPart
                Dim TempString = $"map_{ElementA}_{ElementB}.bin"
                If Not Exclude.Contains(TempString) Then DeletionList.Add(TempString)
                Counter += 1
                BackgroundWorker1.ReportProgress(100 / AllCombB * Counter)
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
        For Each Element In DeletionList
            Try
                If CB2 Then IO.File.Copy(Mappath & Element, Backuppath & Element)
                IO.File.Delete(Mappath & Element)
                DeletedFiles_i += 1
            Catch ex As Exception
                ErrorList_i = ErrorList_i & ex.Message & vbCrLf
                Errors_i += 1
            End Try
            Counter += 1
            BackgroundWorker1.ReportProgress(100 / DeletionList.Count * Counter)
            If BackgroundWorker1.CancellationPending = True Then e.Cancel = True : Exit Sub
        Next

        DeletedFiles = DeletedFiles_i
        Errors = Errors_i
        Errorlist = ErrorList_i
#End Region
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        MessageBox.Show($"{DeletedFiles} have been deleted, {Errors} errors have occurred.", "Process finished")
        If Not Errorlist = Nothing And Not Errorlist = "" Then MessageBox.Show($"Errorlist:{vbCrLf}{Errorlist}")
        If Not ProgressBar1.Value = 100 Then ProgressBar1.Value = 100
        Steps = 0
        Label5.Text = $"Step {Steps}"
        LastPercentage = 100
        Button1.Enabled = True
        Button1.Text = "Go"
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        If LastPercentage > e.ProgressPercentage Then Steps += 1 : Label5.Text = $"Step {Steps}"
        LastPercentage = e.ProgressPercentage
        ProgressBar1.Value = e.ProgressPercentage
    End Sub
End Class
