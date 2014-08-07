Sub UnZip(ByVal myZipFile, ByVal myTargetDir)
    Set fso = CreateObject("Scripting.FileSystemObject")
    If NOT fso.FileExists(myZipFile) Then
        Exit Sub
    ElseIf fso.GetExtensionName(myZipFile) <> "zip" Then
        Exit Sub
    ElseIf NOT fso.FolderExists(myTargetDir) Then
        fso.CreateFolder(myTargetDir)
    End If
    Set objShell = CreateObject("Shell.Application")
    Set objSource = objShell.NameSpace(myZipFile)
    Set objFolderItem = objSource.Items()
    Set objTarget = objShell.NameSpace(myTargetDir)
    intOptions = 256
    objTarget.CopyHere objFolderItem, intOptions
End Sub