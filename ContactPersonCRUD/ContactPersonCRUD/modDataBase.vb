' / --------------------------------------------------------------------------------
' / Developer : Mr.Surapon Yodsanga (Thongkorn Tubtimkrob)
' / eMail : thongkorn@hotmail.com
' / URL: http://www.g2gnet.com (Khon Kaen - Thailand)
' / Facebook: https://www.facebook.com/g2gnet (For Thailand)
' / Facebook: https://www.facebook.com/commonindy (Worldwide)
' / Microsoft Visual Basic .NET (2010)
' /
' / This is open source code under @Copyleft by Thongkorn Tubtimkrob.
' / You can modify and/or distribute without to inform the developer.
' / --------------------------------------------------------------------------------
Imports System.Data.OleDb
Imports Microsoft.VisualBasic

Module modDataBase
    '// Declare variable one time but use many times.
    Public Conn As OleDbConnection
    Public Cmd As OleDbCommand
    Public DS As DataSet
    Public DR As OleDbDataReader
    Public DA As OleDbDataAdapter
    Public strSQL As String '// Major SQL
    Public strStmt As String    '// Minor SQL

    '// Data Path 
    Public strPathData As String = MyPath(Application.StartupPath) & "Data\"
    '// Images Path
    Public strPathImages As String = MyPath(Application.StartupPath) & "Images\"

    Public Sub ConnectDataBase()
        Dim strConn As String = _
            "Provider = Microsoft.ACE.OLEDB.12.0;" & _
            "Data Source = " & strPathData & "Contact.accdb;"
        Try
            Conn = New OleDb.OleDbConnection(strConn)
            '// Test Connection
            Conn.Open()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Report Status", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '// Force end of program.
            End
        End Try
    End Sub

    ' / --------------------------------------------------------------------------------
    ' / Get my project path
    ' / AppPath = C:\My Project\bin\debug
    ' / Replace "\bin\debug" with "\"
    ' / Return : C:\My Project\
    Function MyPath(AppPath As String) As String
        '/ MessageBox.Show(AppPath);
        AppPath = AppPath.ToLower()
        '/ Return Value
        MyPath = AppPath.Replace("\bin\debug", "\").Replace("\bin\release", "\").Replace("\bin\x86\debug", "\")
        '// If not found folder then put the \ (BackSlash) or ASCII Code = 92 at the end.
        If Right(MyPath, 1) <> Chr(92) Then MyPath = MyPath & Chr(92)
    End Function
End Module
