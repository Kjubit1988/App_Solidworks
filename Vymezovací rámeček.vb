Imports System.IO
Imports swDocumentTypes_e = SwConst.swDocumentTypes_e
Imports swOpenDocOptions_e = SwConst.swOpenDocOptions_e
Imports swSaveAsOptions_e = SolidWorks.Interop.swconst.swSaveAsOptions_e
Imports ModelView = SldWorks.ModelView
Imports SldWorks
Imports SolidWorks.Interop.swconst
Imports Aspose.Slides.MathText

Public Class Form1
    Private Const swOpenDocOptions_Silent As SolidWorks.Interop.swconst.swOpenDocOptions_e = swOpenDocOptions_e.swOpenDocOptions_Silent
    Private swModel As SldWorks.ModelDoc2
    Private swSketch As SldWorks.Sketch
    Private swBoundingBox As Object
    Private swApp As SldWorks.SldWorks
    Private swModelView As ModelView

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim dialog As New FolderBrowserDialog

        With dialog
            .Description = "Vyberte adresář"
            .ShowNewFolderButton = False

            If .ShowDialog() = DialogResult.OK Then
                TextBox1.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim strFolderPath As String = TextBox1.Text

        If Directory.Exists(strFolderPath) Then
            swApp = New SldWorks.SldWorks
            swApp.OpenDoc6("", swDocumentTypes_e.swDocNONE, swOpenDocOptions_Silent, "", 0, 0)
            swApp.Visible = True

            ' Získání seznamu souborů ve vybraném adresáři
            Dim fileNames As String() = Directory.GetFiles(strFolderPath, "*.SLDPRT")

            ' Procházení a zpracování jednotlivých souborů
            For Each fileName As String In fileNames
                Dim swModel As SldWorks.ModelDoc2 = swApp.OpenDoc6(fileName, swDocumentTypes_e.swDocPART, swOpenDocOptions_Silent, "", 0, 0)
                Dim swSketch As SolidWorks.Interop.sldworks.Sketch = Nothing
                Dim swBoundingBox As Object = Nothing


                'Zde můžete provést další zpracování souboru, např. manipulaci s modelem

                swBoundingBox = swModel.FeatureManager.InsertGlobalBoundingBox(SwConst.swGlobalBoundingBoxFitOptions_e.swBoundingBoxType_BestFit, False, False, 0)
                swModel.ClearSelection2(True)




                'Tento blok je pro check button -skrytí nebo zobrazení vymezovacího rámečku
                Dim ext As SldWorks.ModelDocExtension = swModel.Extension
                Dim sketch As SldWorks.ModelDoc2
                Dim featMgr As SldWorks.FeatureManager = swModel.FeatureManager




                Dim boolstatus As Boolean
                sketch = swApp.ActiveDoc


                If CheckBox1.Checked Then
                    boolstatus = ext.SelectByID2("Bounding Box", "BBOXSKETCH", 0, 0, 0, False, 0, Nothing, 0)
                    sketch.UnblankSketch()
                Else
                    boolstatus = ext.SelectByID2("Bounding Box", "BBOXSKETCH", 0, 0, 0, False, 0, Nothing, 0)
                    sketch.BlankSketch()

                End If

                swModel.ClearSelection2(True)

                Dim partDoc As SldWorks.PartDoc = CType(swModel, SldWorks.PartDoc)

                ' Select the bounding box sketch
                boolstatus = ext.SelectByID2("Bounding Box", "BBOXSKETCH", 0, 0, 0, False, 0, Nothing, 0)

                Dim boxMinPoint As MathPoint = Nothing
                Dim boxMaxPoint As MathPoint = Nothing
                Dim v = partDoc.GetPartBox(boxMinPoint, boxMaxPoint)

                Dim bbox As Double() = {boxMinPoint.X, boxMinPoint.Y, boxMinPoint.Z, boxMaxPoint.X, boxMaxPoint.Y, boxMaxPoint.Z}

                ' Concatenate the dimensions with "x" between them
                Dim dimensionsString As String = String.Join("x", bbox)

                ' Set the custom property with the concatenated dimensions
                swModel.Extension.CustomPropertyManager("").Add3("BoundingBoxDimensions", swCustomInfoType_e.swCustomInfoText, dimensionsString, swCustomPropertyAddOption_e.swCustomPropertyReplaceValue)

                ' ... (your existing code)
                Dim lErrors As Long
                Dim lwarnings As Long

                swModel.Save3(swSaveAsOptions_e.swSaveAsOptions_Silent, lErrors, lwarnings)
                'Error
                Debug.Print("Errors as defined in swFileSaveError_e: " & lErrors)
                ' Warnings
                Debug.Print("Warnings as defined in swFileSaveWarning_e: " & lwarnings)
                'swModel.Close()
                swApp.QuitDoc(swModel.GetTitle())


            Next

            ' Ukončení instance SOLIDWORKS,

            swApp.ExitApp()
            swApp = Nothing
            'Oznámení o uspěšném ukončení
            MessageBox.Show("Úspešně dokončeno")
        Else
            MessageBox.Show("Zadejte prosím platný adresář.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        'If swModel IsNot Nothing AndAlso swBoundingBox IsNot Nothing Then
        'If CheckBox1.Checked Then
        'swBoundingBox.Visible = True

        ' Else
        'swBoundingBox.Visible = False

        '  End If

        'swModel.GraphicsRedraw2()
        ' End If 
    End Sub
End Class

