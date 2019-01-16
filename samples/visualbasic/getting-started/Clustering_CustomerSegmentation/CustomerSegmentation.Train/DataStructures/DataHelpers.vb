Namespace CustomerSegmentation.DataStructures
    Public Class DataHelpers
        Public Shared Function PreProcessAndSave(offersDataLocation As String, transactionsDataLocation As String, pivotDataLocation As String) As IEnumerable(Of PivotData)
            Dim preProcessData = PreProcess(offersDataLocation, transactionsDataLocation)
            PivotData.SaveToCsv(preProcessData, pivotDataLocation)
            Return preProcessData
        End Function

        Public Shared Function PreProcess(offersDataLocation As String, transactionsDataLocation As String) As IEnumerable(Of PivotData)
            Common.ConsoleHelper.ConsoleWriteHeader("Preprocess input files")
            Console.WriteLine($"Offers file: {offersDataLocation}")
            Console.WriteLine($"Transactions file: {transactionsDataLocation}")

            Dim offers = Offer.ReadFromCsv(offersDataLocation)
            Dim transactions = Transaction.ReadFromCsv(transactionsDataLocation)

            ' inner join datasets
            Dim clusterData = (
                From [of] In offers
                Join tr In transactions On [of].OfferId Equals tr.OfferId
                Select New With {
                    Key [of].OfferId,
                    Key [of].Campaign,
                    Key [of].Discount,
                    Key tr.LastName,
                    Key [of].LastPeak,
                    Key [of].Minimum,
                    Key [of].Origin,
                    Key [of].Varietal,
                    Key .Count = 1
                }).ToArray()

            ' pivot table (naive way)
            ' based on code from https://stackoverflow.com/a/43091570
            Dim pivotDataArray = (
                From c In clusterData
                Group c By c.LastName Into gcs = Group
                Let lookup = gcs.ToLookup(Function(y) y.OfferId, Function(y) y.Count)
                Select New PivotData() With {
                    .LastName = LastName,
                    .C1 = CSng(lookup("1").Sum()),
                    .C2 = CSng(lookup("2").Sum()),
                    .C3 = CSng(lookup("3").Sum()),
                    .C4 = CSng(lookup("4").Sum()),
                    .C5 = CSng(lookup("5").Sum()),
                    .C6 = CSng(lookup("6").Sum()),
                    .C7 = CSng(lookup("7").Sum()),
                    .C8 = CSng(lookup("8").Sum()),
                    .C9 = CSng(lookup("9").Sum()),
                    .C10 = CSng(lookup("10").Sum()),
                    .C11 = CSng(lookup("11").Sum()),
                    .C12 = CSng(lookup("12").Sum()),
                    .C13 = CSng(lookup("13").Sum()),
                    .C14 = CSng(lookup("14").Sum()),
                    .C15 = CSng(lookup("15").Sum()),
                    .C16 = CSng(lookup("16").Sum()),
                    .C17 = CSng(lookup("17").Sum()),
                    .C18 = CSng(lookup("18").Sum()),
                    .C19 = CSng(lookup("19").Sum()),
                    .C20 = CSng(lookup("20").Sum()),
                    .C21 = CSng(lookup("21").Sum()),
                    .C22 = CSng(lookup("22").Sum()),
                    .C23 = CSng(lookup("23").Sum()),
                    .C24 = CSng(lookup("24").Sum()),
                    .C25 = CSng(lookup("25").Sum()),
                    .C26 = CSng(lookup("26").Sum()),
                    .C27 = CSng(lookup("27").Sum()),
                    .C28 = CSng(lookup("28").Sum()),
                    .C29 = CSng(lookup("29").Sum()),
                    .C30 = CSng(lookup("30").Sum()),
                    .C31 = CSng(lookup("31").Sum()),
                    .C32 = CSng(lookup("32").Sum())
                }).ToArray()

            Console.WriteLine($"Total rows: {pivotDataArray.Length}")

            Return pivotDataArray
        End Function
    End Class
End Namespace
