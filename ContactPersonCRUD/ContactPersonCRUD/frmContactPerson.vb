' / --------------------------------------------------------------------------------
' / Developer : Mr.Surapon Yodsanga (Thongkorn Tubtimkrob)
' / eMail : thongkorn@hotmail.com
' / URL: http://www.g2gnet.com (Khon Kaen - Thailand)
' / Facebook: https://www.facebook.com/g2gnet (For Thailand)
' / Facebook: https://www.facebook.com/commonindy (Worldwide)
' / More Info: http://www.g2gsoft.com
' /
' / Purpose: Keep personal information within the workplace.
' / Microsoft Visual Basic .NET (2010) & MS Access 2007+
' /
' / This is open source code under @CopyLeft by Thongkorn Tubtimkrob.
' / You can modify and/or distribute without to inform the developer.
' / --------------------------------------------------------------------------------
Imports System.Data.OleDb
Imports System.Drawing
Imports System.IO

Public Class frmContactPerson
    Dim PK As Integer   '// Primary Key
    Dim NewData As Boolean = False  '// 
    '//
    Dim newFileName As String   '// File name of Image (New)
    Dim orgPicName As String    '// Orginal of Image
    Dim streamPic As Stream     '// Use Steam instead IO.

    Private Sub frmContactPerson_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dim Result As Byte = MessageBox.Show("Are you sure you want to exit the program?", "Confirm closing program", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
        If Result = DialogResult.Yes Then
            Me.Dispose()
            If Conn.State = ConnectionState.Open Then Conn.Close()
            GC.SuppressFinalize(Me)
            Application.Exit()
        Else
            e.Cancel = True
        End If
    End Sub

    ' / --------------------------------------------------------------------------------
    Private Sub frmContactPerson_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        '// Connect MS Access DataBase
        Call ConnectDataBase()
        lblRecordCount.Text = ""
        Call NewMode()
        Call SetupDGVData()
        Call RetrieveData()
        '// Show picture
        chkPicture.Checked = True
    End Sub

    ' / --------------------------------------------------------------------------------
    '// Initialize DataGridView @Run Time
    Private Sub SetupDGVData()
        With dgvData
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .ReadOnly = True
            .Font = New Font("Tahoma", 9)
            ' Columns Specified
            .Columns.Add("ContactPK", "ContactPK")
            .Columns.Add("Fullname", "Full name")
            .Columns.Add("Nickname", "Nickname")
            .Columns.Add("PositionName", "Position")
            .Columns.Add("DepartmentName", "Department")
            .Columns.Add("Mobile", "Mobile")
            .Columns.Add("Phone", "Phone Ext.")
            .Columns.Add("Email", "Email")
            .Columns.Add("LineID", "Line")
            .Columns.Add("FacebookID", "Facebook")
            '// Column Picture
            Dim colPicture As New DataGridViewImageColumn
            .Columns.Add(colPicture)
            With colPicture
                .HeaderText = "Picture"
                .Name = "PictureName"
                .ImageLayout = DataGridViewImageCellLayout.Stretch
            End With
            '//
            .Columns.Add("Note", "Note")
            '// Hidden Columns
            .Columns(0).Visible = False
            .Columns(7).Visible = False
            .Columns(8).Visible = False
            .Columns(9).Visible = False
            '// PictureName
            .Columns("PictureName").Visible = True
            .Columns("Note").Visible = False
            ' Autosize Column
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AutoResizeColumns()
            '// Even-Odd Color
            .AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue
            ' Adjust Header Styles
            With .ColumnHeadersDefaultCellStyle
                .BackColor = Color.Navy
                .ForeColor = Color.Black ' Color.White
                .Font = New Font("Tahoma", 9, FontStyle.Bold)
            End With
        End With
    End Sub

    ' / --------------------------------------------------------------------------------
    ' / Collect all searches and impressions. Come in the same place
    ' / blnSearch = True, Show that the search results.
    ' / blnSearch is set to False, Show all records.
    Private Sub RetrieveData(Optional ByVal blnSearch As Boolean = False)
        strSQL = _
            " SELECT tblContact.ContactPK, tblContact.Fullname, tblContact.Nickname, tblContact.Mobile, " & _
            " tblContact.Phone, tblContact.eMail, tblContact.LineID, tblContact.FacebookID, " & _
            " tblContact.PictureName, tblContact.Note, " & _
            " tblPosition.PositionName, tblDepartment.DepartmentName " & _
            " FROM [tblPosition] INNER JOIN (tblDepartment INNER JOIN tblContact ON " & _
            " tblDepartment.DepartmentPK = tblContact.DepartmentFK) ON tblPosition.PositionPK = tblContact.PositionFK "

        '// blnSearch = True for Serach
        If blnSearch Then
            strSQL = strSQL & _
                " WHERE " & _
                " [Fullname] " & " Like '%" & txtSearch.Text & "%'" & " OR " & _
                " [Nickname] " & " Like '%" & txtSearch.Text & "%'" & " OR " & _
                " [PositionName] " & " Like '%" & txtSearch.Text & "%'" & " OR " & _
                " [DepartmentName] " & " Like '%" & txtSearch.Text & "%'" & " OR " & _
                " [Mobile] " & " Like '%" & txtSearch.Text & "%'" & " OR " & _
                " [Phone] " & " Like '%" & txtSearch.Text & "%'" & " OR " & _
                " [eMail] " & " Like '%" & txtSearch.Text & "%'" & " OR " & _
                " [LineID] " & " Like '%" & txtSearch.Text & "%'" & " OR " & _
                " [FaceBookID] " & " Like '%" & txtSearch.Text & "%'" & _
                " ORDER BY ContactPK "
        Else
            strSQL = strSQL & " ORDER BY ContactPK "
        End If
        '//
        Try
            Cmd = New OleDbCommand
            If Conn.State = ConnectionState.Closed Then Conn.Open()
            Cmd.Connection = Conn
            Cmd.CommandText = strSQL
            Dim DR As OleDbDataReader = Cmd.ExecuteReader
            Dim i As Long = dgvData.RowCount
            While DR.Read
                With dgvData
                    .Rows.Add(i)
                    .Rows(i).Cells(0).Value = DR.Item("ContactPK").ToString
                    .Rows(i).Cells(1).Value = DR.Item("Fullname").ToString
                    .Rows(i).Cells(2).Value = DR.Item("Nickname").ToString
                    .Rows(i).Cells(3).Value = DR.Item("PositionName").ToString
                    .Rows(i).Cells(4).Value = DR.Item("DepartmentName").ToString
                    .Rows(i).Cells(5).Value = DR.Item("Mobile").ToString
                    .Rows(i).Cells(6).Value = DR.Item("Phone").ToString
                    .Rows(i).Cells(7).Value = DR.Item("eMail").ToString
                    .Rows(i).Cells(8).Value = DR.Item("LineID").ToString
                    .Rows(i).Cells(9).Value = DR.Item("FaceBookID").ToString
                    '// Show picture in cell.
                    If DR.Item("PictureName").ToString <> "" Then
                        '//dgvData.Rows(i).Height = 75
                        '// Column 10 = "PictureName"
                        dgvData.Columns(10).Width = 75
                        '// First, before load data into DataGrid and check File exists or not?
                        If Dir(strPathImages & DR.Item("PictureName").ToString) = "" Then
                            '// strPathImages in modDataBase.vb
                            dgvData.Rows(i).Cells(10).Value = Image.FromFile(strPathImages & "people.png")
                        Else
                            dgvData.Rows(i).Cells(10).Value = Image.FromFile(strPathImages & DR.Item("PictureName").ToString)
                        End If
                    Else
                        dgvData.Rows(i).Cells(10).Value = Image.FromFile(strPathImages & "people.png")
                        '//dgvData.Rows(i).Height = 75
                        dgvData.Columns(10).Width = 75
                    End If
                    newFileName = DR.Item("PictureName").ToString
                    ' / --------------------------------------------------------------------------------
                    '// Keep picture's name into TAG for each cell in DataGrid.
                    '// Used when to display in PictureBox.
                    dgvData.Rows(i).Cells(10).Tag = DR.Item("PictureName").ToString
                    ' / --------------------------------------------------------------------------------
                    '//
                    .Rows(i).Cells(11).Value = DR.Item("Note").ToString
                End With
                i += 1
            End While
            newFileName = Nothing   '// Clear value
            lblRecordCount.Text = "[Total : " & dgvData.RowCount & " records]"
            DR.Close()
            '// Adjust row height.
            If chkPicture.Checked Then
                dgvData.Columns("PictureName").Visible = True
                '// Jump to sub program
                Call AdjustRowHeight(75)
            Else
                dgvData.Columns("PictureName").Visible = False
                Call AdjustRowHeight(28)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        '//
        txtSearch.Clear()
    End Sub

    ' / --------------------------------------------------------------------------------
    ' / Double click to edit item.
    Private Sub dgvData_DoubleClick(sender As Object, e As System.EventArgs) Handles dgvData.DoubleClick
        '// If you add / edit information should be reminded before.
        If btnDelete.Text = "Cancel" Then
            txtFullname.Focus()
            Exit Sub
        End If
        '//
        If dgvData.RowCount <= 0 Then Return
        '// Read the value of the focus row.
        Dim iRow As Integer = dgvData.CurrentRow.Index
        '// 
        PK = dgvData.Item(0, iRow).Value  '// Keep Primary Key
        '// If you share a file, you need to refresh the data.
        strSQL = _
            " SELECT tblContact.ContactPK, tblContact.Fullname, tblContact.Nickname, tblContact.Mobile, " & _
            " tblContact.Phone, tblContact.eMail, tblContact.LineID, tblContact.FacebookID, " & _
            " tblContact.PictureName, tblContact.Note, " & _
            " tblPosition.PositionName, tblDepartment.DepartmentName " & _
            " FROM [tblPosition] INNER JOIN (tblDepartment INNER JOIN tblContact ON " & _
            " tblDepartment.DepartmentPK = tblContact.DepartmentFK) ON tblPosition.PositionPK = tblContact.PositionFK " & _
            " WHERE ContactPK = " & PK
        If Conn.State = ConnectionState.Closed Then Conn.Open()
        DA = New OleDbDataAdapter(strSQL, Conn)
        DS = New DataSet
        DA.Fill(DS)
        '/ ------------------------------------------------------------------
        With DS.Tables(0)
            txtFullname.Text = "" & .Rows(0)("Fullname").ToString()
            '// Keep the original value for later comparison.
            txtFullname.Tag = txtFullname.Text
            '// Using Double quote "" for trap error null value
            txtNickname.Text = "" & .Rows(0)("Nickname").ToString()
            ' / --------------------------------------------------------------------------------
            '// Load data Detail Table into ComboBox
            Call PopulateComboBox(cmbPosition, "tblPosition", "PositionName", "PositionPK")
            cmbPosition.Text = "" & dgvData.Item(3, iRow).Value
            Call PopulateComboBox(cmbDepartment, "tblDepartment", "DepartmentName", "DepartmentPK")
            cmbDepartment.Text = "" & dgvData.Item(4, iRow).Value
            '//
            txtMobile.Text = "" & .Rows(0)("Mobile").ToString()
            txtPhone.Text = "" & .Rows(0)("Phone").ToString()
            txtEMail.Text = "" & .Rows(0)("EMail").ToString()
            txtLineID.Text = "" & .Rows(0)("LineID").ToString()
            txtFacebookID.Text = "" & .Rows(0)("FacebookID").ToString()
            txtNote.Text = "" & .Rows(0)("Note").ToString()
            '// Load Picture
            Call ShowPicture(.Rows(0)("PictureName").ToString)
        End With
        DS.Dispose()
        DA.Dispose()
        '// Change to Edit Mode
        NewData = False
        EditMode()
    End Sub

    ' / --------------------------------------------------------------------------------
    ' / You can press enter to select row.
    Private Sub dgvData_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles dgvData.KeyDown
        If e.KeyCode = Keys.Enter Then
            Call dgvData_DoubleClick(sender, e)
            ' No move to next row.
            e.SuppressKeyPress = True
        End If
    End Sub

    ' / --------------------------------------------------------------------------------
    ' / Load table detail into ComboBox
    Public Sub PopulateComboBox(cmbCtrl As ComboBox, strTable As String, strFieldName As String, Optional ByVal strFieldPK As String = vbNullString)
        Try
            If Conn.State = ConnectionState.Closed Then Conn.Open()
            strStmt = "SELECT * FROM " & strTable & " ORDER BY " & strFieldName
            Cmd = New OleDb.OleDbCommand(strStmt, Conn)
            DR = Cmd.ExecuteReader
            Dim DT As DataTable = New DataTable
            DT.Load(DR)
            '/ Primary Key (ValueMember)
            cmbCtrl.ValueMember = strFieldPK
            '/ Display the name
            cmbCtrl.DisplayMember = strFieldName
            cmbCtrl.DataSource = DT
            '// Autocomplete
            With cmbCtrl
                .DropDownStyle = ComboBoxStyle.DropDown
                .AutoCompleteMode = AutoCompleteMode.Suggest
                .AutoCompleteSource = AutoCompleteSource.ListItems
            End With
            DR.Close()
            Conn.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ' / --------------------------------------------------------------------------------
    ' / Add New Mode
    ' / --------------------------------------------------------------------------------
    Private Sub NewMode()
        '// Clear all TextBox.
        For Each c In GroupBox1.Controls
            If TypeOf c Is TextBox Then
                DirectCast(c, TextBox).Clear()
                DirectCast(c, TextBox).Enabled = False
            End If
        Next
        '// Clear all ComboBox
        For Each cbo In GroupBox1.Controls.OfType(Of ComboBox)()
            cbo.Enabled = False
        Next
        '//
        btnAdd.Enabled = True
        btnSave.Enabled = False
        btnDelete.Enabled = True
        btnDelete.Text = "Delete"
        btnExit.Enabled = True
        '//
        btnBrowse.Enabled = False
        btnDeleteImg.Enabled = False
        picData.Image = Image.FromFile(strPathImages & "people.png")
    End Sub

    ' / --------------------------------------------------------------------------------
    ' / Edit Data Mode
    Private Sub EditMode()
        '// Clear all TextBox
        For Each c In GroupBox1.Controls
            If TypeOf c Is TextBox Then
                DirectCast(c, TextBox).Enabled = True
            End If
        Next
        '// Clear all ComboBox
        For Each cbo In GroupBox1.Controls.OfType(Of ComboBox)()
            cbo.Enabled = True
        Next
        btnAdd.Enabled = False
        btnSave.Enabled = True
        btnDelete.Enabled = True
        btnDelete.Text = "Cancel"
        btnExit.Enabled = False
        '//
        btnBrowse.Enabled = True
        btnDeleteImg.Enabled = True
    End Sub

    ' / --------------------------------------------------------------------------------
    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click
        '// Validate Data
        If txtFullname.Text = "" Or IsNothing(txtFullname.Text) Or txtFullname.Text.Length = 0 Then
            MessageBox.Show("Full name cannot be empty.", "Report Status", _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFullname.Focus()
            Exit Sub
        End If
        '//
        '// If the new value (Text) with the original value (Tag) is not equal, then the value changed in field "Fullname"
        If txtFullname.Text.ToLower <> LCase(txtFullname.Tag) Then
            strSQL = _
            " SELECT Count(tblContact.Fullname) AS CountFullname FROM tblContact " & _
            " WHERE Fullname = " & "'" & txtFullname.Text & "'"
            If DuplicateName(strSQL) Then
                MessageBox.Show("Duplicate Full name, please enter new value.", "Report Status", _
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFullname.Focus()
                Exit Sub
            End If
        End If
        '// NewData = True, It's Add New Mode
        If NewData Then
            strSQL = _
                " INSERT INTO tblContact(" & _
                " ContactPK, Fullname, Nickname, PositionFK, DepartmentFK, Mobile, Phone, Email, LineID, FacebookID, " & _
                " PictureName, [Note], " & _
                " DateAdded, DateModified) " & _
                " VALUES(" & _
                "'" & SetupNewPK() & "'," & _
                "'" & txtFullname.Text & "'," & _
                "'" & txtNickname.Text & "'," & _
                "'" & PKComboBox(cmbPosition, "tblPosition", "PositionName", "PositionPK") & "'," & _
                "'" & PKComboBox(cmbDepartment, "tblDepartment", "DepartmentName", "DepartmentPK") & "'," & _
                "'" & txtMobile.Text & "'," & _
                "'" & txtPhone.Text & "'," & _
                "'" & txtEMail.Text & "'," & _
                "'" & txtLineID.Text & "'," & _
                "'" & txtFacebookID.Text & "'," & _
                "'" & GetFileImages() & "'," & _
                "'" & txtNote.Text & "'," & _
                "'" & Now.ToString("dd/MM/yyyy") & "'," & _
                "'" & Now.ToString("dd/MM/yyyy") & "'" & _
                ")"
            '// EDIT MODE
        Else
            '// START UPDATE
            strSQL = _
                " UPDATE tblContact SET " & _
                " [Fullname]='" & txtFullname.Text & "', " & _
                " [Nickname]='" & txtNickname.Text & "', " & _
                " [PositionFK]=" & PKComboBox(cmbPosition, "tblPosition", "PositionName", "PositionPK") & ", " & _
                " [DepartmentFK]=" & PKComboBox(cmbDepartment, "tblDepartment", "DepartmentName", "DepartmentPK") & ", " & _
                " [Mobile]='" & txtMobile.Text & "', " & _
                " [Phone]='" & txtPhone.Text & "', " & _
                " [Email]='" & txtEMail.Text & "', " & _
                " [LineID]='" & txtLineID.Text & "', " & _
                " [FaceBookID]='" & txtFacebookID.Text & "', " & _
                " [PictureName]='" & GetFileImages() & "', " & _
                " [Note]='" & txtNote.Text & "', " & _
                " [DateModified]='" & Now.ToString("dd/MM/yyyy") & "'" & _
                " WHERE ContactPK = " & PK & ""
        End If
        '// Insert or Update same as operation
        If DoSQL(strSQL) Then
            MessageBox.Show("Records Updated Completed.", "Update Status", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        '//
        cmbPosition.SelectedIndex = 0
        cmbDepartment.SelectedIndex = 0

        '// Clear rows in DataGridView
        dgvData.Rows.Clear()
        '// Refresh DataGridView
        Call RetrieveData()
        '// Add New Mode
        Call NewMode()
    End Sub

    ' / --------------------------------------------------------------------------------
    '// UPDATE DATA
    Private Function DoSQL(ByVal Sql As String) As Boolean
        Cmd = New OleDb.OleDbCommand
        If Conn.State = ConnectionState.Closed Then Conn.Open()
        'MsgBox(Sql)
        Try
            Cmd.Connection = Conn
            Cmd.CommandType = CommandType.Text
            Cmd.CommandText = Sql
            Cmd.ExecuteNonQuery()
            Cmd.Dispose()
            Return True
        Catch ex As Exception
            MsgBox("Error Update: " & ex.Message)
            Return False
        End Try
    End Function

    ' / --------------------------------------------------------------------------------
    ' / Get Filename + Extesnsion only, not Path
    Private Function GetFileImages() As String
        '// Get the Filename + Extension only
        Dim iArr() As String
        iArr = Split(newFileName, "\")
        GetFileImages = iArr(UBound(iArr))
        '//
        '// If same original and new
        If orgPicName = newFileName Then Return GetFileImages
        Try
            '// ------------- Copy File -------------
            '// Determine whether the source file is real or not.
            If System.IO.File.Exists(newFileName) = True Then
                ' Trap Error in the case source = destination
                If LCase(strPathImages + GetFileImages) <> LCase(newFileName) Then
                    '// Copy the Source file (newFileName) to the Destination (DestFile). 
                    '// If the same file is found, overwrite (OverWrite = True).
                    System.IO.File.Copy(newFileName, strPathImages + GetFileImages, True)
                End If
            End If
            newFileName = Nothing
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Function

    ' / --------------------------------------------------------------------------------
    ' / Function to find and create the new Primary Key not to duplicate.
    Function SetupNewPK() As Long
        strSQL = _
            " SELECT MAX(tblContact.ContactPK) AS MaxPK FROM tblContact "
        If Conn.State = ConnectionState.Closed Then Conn.Open()
        Cmd = New OleDb.OleDbCommand(strSQL, Conn)
        '/ Check if the information is available. And return it back
        If IsDBNull(Cmd.ExecuteScalar) Then
            '// Start at 1
            SetupNewPK = 1
        Else
            SetupNewPK = Cmd.ExecuteScalar + 1
        End If
    End Function

    ' / --------------------------------------------------------------------------------
    ' / This example uses the Fullname validation.
    Public Function DuplicateName(ByVal Sql As String) As Boolean
        If Conn.State = ConnectionState.Closed Then Conn.Open()
        Cmd = New OleDb.OleDbCommand(Sql, Conn)
        '// Return count records
        DuplicateName = Cmd.ExecuteScalar
    End Function

    ' / --------------------------------------------------------------------------------
    ' / Function insert new data in Detail Table and return Primary Key for Master Table.
    Function PKComboBox(cmbCtrl As ComboBox, strTable As String, strFieldName As String, Optional ByVal strFieldPK As String = vbNullString) As Integer
        '// If ComboBox is blank data then return 1 (Blank Data)
        '// PKComboBox = 1
        If cmbCtrl.Text = "" Or cmbCtrl.Text.Length = 0 Or IsDBNull(cmbCtrl.Text) Then Return 1
        '//
        strSQL = _
            "SELECT * FROM " & strTable & " WHERE " & strFieldName & " = " & "'" & cmbCtrl.Text & "'"
        If Conn.State = ConnectionState.Closed Then Conn.Open()
        Cmd = New OleDb.OleDbCommand(strSQL, Conn)
        '// Get the Primary Key
        Dim cmbPK As Integer = Cmd.ExecuteScalar
        '// If not have in Detail Table
        If cmbPK <= 0 Then
            strStmt = _
                " SELECT MAX(" & strFieldPK & ") AS MaxPK FROM " & strTable
            If Conn.State = ConnectionState.Closed Then Conn.Open()
            Cmd = New OleDb.OleDbCommand(strStmt, Conn)
            '// Increment Primary Key with 1, and Return this value.
            PKComboBox = Cmd.ExecuteScalar + 1
            '/ Add New Data in Detail Table
            Try
                Using Comm As New OleDb.OleDbCommand()
                    With Comm
                        .Connection = Conn
                        .CommandType = CommandType.Text
                        .CommandText = _
                            " INSERT INTO " & strTable & " (" & strFieldName & ", " & strFieldPK & ") VALUES (@DName, @DPK)"
                        With .Parameters
                            .Add("@DName", OleDbType.VarChar).Value = cmbCtrl.Text
                            '/ ------------------------------------------------------------------
                            .Add("@DPK", OleDbType.Integer).Value = PKComboBox
                            '/ ------------------------------------------------------------------
                        End With
                        ' Insert new record.
                        .ExecuteNonQuery()
                        .Parameters.Clear()
                        '/ ------------------------------------------------------------------
                    End With
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Else
            '// Return
            PKComboBox = cmbPK
        End If
        Cmd.Dispose()
    End Function

    '/ ------------------------------------------------------------------
    '// Load Detail Table into ComboBox
    Private Sub btnAdd_Click(sender As System.Object, e As System.EventArgs) Handles btnAdd.Click
        NewData = True  '// Add New Mode
        Call EditMode()
        '// Load Detail Table into ComboBox
        Call PopulateComboBox(cmbPosition, "tblPosition", "PositionName", "PositionPK")
        Call PopulateComboBox(cmbDepartment, "tblDepartment", "DepartmentName", "DepartmentPK")
        '//
        picData.Image = Image.FromFile(strPathImages & "people.png")
        txtFullname.Focus()
    End Sub

    '// Load Detail Table into ComboBox
    Private Sub btnDelete_Click(sender As System.Object, e As System.EventArgs) Handles btnDelete.Click
        '// If Edit Data Mode
        If btnDelete.Text = "Cancel" Then
            btnAdd.Enabled = True
            btnSave.Enabled = True
            btnDelete.Enabled = True
            btnDelete.Text = "Delete"
            btnExit.Enabled = True
            '//
            btnBrowse.Enabled = False
            btnDeleteImg.Enabled = False
            '//
            cmbPosition.SelectedIndex = -1
            cmbDepartment.SelectedIndex = -1
            picData.Image = Image.FromFile(strPathImages & "people.png")
            NewMode()
        Else
            If dgvData.RowCount = 0 Then Exit Sub
            '// Receive Primary Key value to confirm the deletion.
            Dim iRow As Long = dgvData.Item(0, dgvData.CurrentRow.Index).Value
            Dim FName As String = dgvData.Item(1, dgvData.CurrentRow.Index).Value
            Dim Result As Byte = MessageBox.Show("Are you sure you want to delete the data?" & vbCrLf & "Full Name: " & FName, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
            If Result = DialogResult.Yes Then
                '// iRow is the ContactPK or Primary key that is hidden.
                strStmt = " DELETE FROM tblContact WHERE ContactPK = " & iRow
                '// UPDATE RECORD
                DoSQL(strStmt)
                '//
                '// Check if use default image.
                If UCase(InStrRev(strPathImages & dgvData.Item(10, dgvData.CurrentRow.Index).ToString, "\")) <> "people.png" Then
                    '// Remove original picture from Column 10 in DataGridView (strPathImages from modDataBase.vb)
                    Dim strPicName As String = strPathImages & dgvData.Item(10, dgvData.CurrentRow.Index).ToString
                    If strPicName IsNot Nothing Then
                        If System.IO.File.Exists(strPicName) = True Then
                            System.IO.File.Delete(strPicName)
                            strPicName = Nothing
                        End If
                    End If
                End If
                '//
                Call NewMode()
                '//
                dgvData.Rows.Clear()
                Call RetrieveData()
            End If
        End If
    End Sub

    Private Sub btnRefresh_Click(sender As System.Object, e As System.EventArgs) Handles btnRefresh.Click
        dgvData.Rows.Clear()
        Call RetrieveData()
    End Sub

    Private Sub txtSearch_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtSearch.KeyPress
        '// Undesirable characters for the database ex.  ', * or %
        txtSearch.Text = Replace(Trim(txtSearch.Text), "'", "").Replace("%", "").Replace("*", "")
        If Trim(txtSearch.Text) = "" Or Len(Trim(txtSearch.Text)) = 0 Then Exit Sub
        ' RetrieveData(True) It means searching for information.
        If e.KeyChar = Chr(13) Then '// Press Enter
            '// No beep.
            e.Handled = True
            '//
            dgvData.Rows.Clear()
            Call RetrieveData(True)
        End If
    End Sub

    Private Sub btnBrowse_Click(sender As System.Object, e As System.EventArgs) Handles btnBrowse.Click
        Dim dlgImage As OpenFileDialog = New OpenFileDialog()

        ' / Open File Dialog
        With dlgImage
            .InitialDirectory = strPathImages
            .Title = "Select images"
            .Filter = "Images types (*.jpg;*.png;*.gif;*.bmp)|*.jpg;*.png;*.gif;*.bmp"
            .FilterIndex = 1
            .RestoreDirectory = True
        End With
        ' Select OK after Browse ...
        If dlgImage.ShowDialog() = DialogResult.OK Then
            '// New Image
            newFileName = dlgImage.FileName
            picData.Image = Image.FromFile(newFileName)
        End If
    End Sub

    Private Sub btnDeleteImg_Click(sender As System.Object, e As System.EventArgs) Handles btnDeleteImg.Click
        If orgPicName = "" Or orgPicName.Length = 0 Then Return
        '//
        picData.Image = Image.FromFile(strPathImages & "people.png")
        newFileName = ""
    End Sub

    ' / -----------------------------------------------------------------------------
    ' / Use Steam instead IO.
    ' / -----------------------------------------------------------------------------
    Sub ShowPicture(PicName As String)
        Dim imgDB As Image
        ' Get the name of the image file from the database.
        If PicName.ToString <> "" Then
            ' Verify that the image file meets the specified location.
            If System.IO.File.Exists(strPathImages & PicName.ToString) Then
                ' Because when deleting the image file is locked, it can not be removed.
                ' The file is closed after the image is loaded, so you can delete the file if you need to
                streamPic = File.OpenRead(strPathImages & PicName.ToString)
                imgDB = Image.FromStream(streamPic)
                picData.Image = imgDB
                ' Keep the original image file name. If it is recorded, it will be removed.
                orgPicName = strPathImages & PicName.ToString
                newFileName = orgPicName
            Else
                ' No images were retrieved from the database.
                streamPic = File.OpenRead(strPathImages & "people.png")
                imgDB = Image.FromStream(streamPic)
                picData.Image = imgDB
                ' Keep image filename blank.
                orgPicName = ""
                newFileName = ""
            End If
            ' Is null
        Else
            streamPic = File.OpenRead(strPathImages & "people.png")
            imgDB = Image.FromStream(streamPic)
            picData.Image = imgDB
            ' Keep image filename blank.
            orgPicName = ""
            newFileName = ""

        End If
        '//
        streamPic.Dispose()
        DR.Close()
        Cmd.Dispose()
        Conn.Close()
    End Sub

    Private Sub btnExit_Click(sender As System.Object, e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    ' / -----------------------------------------------------------------------------
    ' / Show picture or not?
    ' / -----------------------------------------------------------------------------
    Private Sub chkPicture_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkPicture.CheckedChanged
        If chkPicture.Checked Then
            dgvData.Columns("PictureName").Visible = True
            Call AdjustRowHeight(75)
        Else
            dgvData.Columns("PictureName").Visible = False
            Call AdjustRowHeight(28)
        End If
    End Sub

    ' / -----------------------------------------------------------------------------
    ' / Change the height of the rows.
    ' / -----------------------------------------------------------------------------
    Private Sub AdjustRowHeight(h As Integer)
        For i As Integer = 0 To dgvData.Rows.Count - 1
            dgvData.Rows(i).Height = h
        Next
    End Sub
End Class
