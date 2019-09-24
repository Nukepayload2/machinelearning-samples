Imports Microsoft.ML.Data

Namespace WebRanking.DataStructures
	Public Class SearchResultData
		<ColumnName("Label"), LoadColumn(0)>
		Public Property Label As UInteger


		<ColumnName("GroupId"), LoadColumn(1)>
		Public Property GroupId As UInteger


		<ColumnName("CoveredQueryTermNumberAnchor"), LoadColumn(2)>
		Public Property CoveredQueryTermNumberAnchor As Single


		<ColumnName("CoveredQueryTermNumberTitle"), LoadColumn(3)>
		Public Property CoveredQueryTermNumberTitle As Single


		<ColumnName("CoveredQueryTermNumberUrl"), LoadColumn(4)>
		Public Property CoveredQueryTermNumberUrl As Single


		<ColumnName("CoveredQueryTermNumberWholeDocument"), LoadColumn(5)>
		Public Property CoveredQueryTermNumberWholeDocument As Single


		<ColumnName("CoveredQueryTermNumberBody"), LoadColumn(6)>
		Public Property CoveredQueryTermNumberBody As Single


		<ColumnName("CoveredQueryTermRatioAnchor"), LoadColumn(7)>
		Public Property CoveredQueryTermRatioAnchor As Single


		<ColumnName("CoveredQueryTermRatioTitle"), LoadColumn(8)>
		Public Property CoveredQueryTermRatioTitle As Single


		<ColumnName("CoveredQueryTermRatioUrl"), LoadColumn(9)>
		Public Property CoveredQueryTermRatioUrl As Single


		<ColumnName("CoveredQueryTermRatioWholeDocument"), LoadColumn(10)>
		Public Property CoveredQueryTermRatioWholeDocument As Single


		<ColumnName("CoveredQueryTermRatioBody"), LoadColumn(11)>
		Public Property CoveredQueryTermRatioBody As Single


		<ColumnName("StreamLengthAnchor"), LoadColumn(12)>
		Public Property StreamLengthAnchor As Single


		<ColumnName("StreamLengthTitle"), LoadColumn(13)>
		Public Property StreamLengthTitle As Single


		<ColumnName("StreamLengthUrl"), LoadColumn(14)>
		Public Property StreamLengthUrl As Single


		<ColumnName("StreamLengthWholeDocument"), LoadColumn(15)>
		Public Property StreamLengthWholeDocument As Single


		<ColumnName("StreamLengthBody"), LoadColumn(16)>
		Public Property StreamLengthBody As Single


		<ColumnName("IdfAnchor"), LoadColumn(17)>
		Public Property IdfAnchor As Single


		<ColumnName("IdfTitle"), LoadColumn(18)>
		Public Property IdfTitle As Single


		<ColumnName("IdfUrl"), LoadColumn(19)>
		Public Property IdfUrl As Single


		<ColumnName("IdfWholeDocument"), LoadColumn(20)>
		Public Property IdfWholeDocument As Single


		<ColumnName("IdfBody"), LoadColumn(21)>
		Public Property IdfBody As Single


		<ColumnName("SumTfAnchor"), LoadColumn(22)>
		Public Property SumTfAnchor As Single


		<ColumnName("SumTfTitle"), LoadColumn(23)>
		Public Property SumTfTitle As Single


		<ColumnName("SumTfUrl"), LoadColumn(24)>
		Public Property SumTfUrl As Single


		<ColumnName("SumTfWholeDocument"), LoadColumn(25)>
		Public Property SumTfWholeDocument As Single


		<ColumnName("SumTfBody"), LoadColumn(26)>
		Public Property SumTfBody As Single


		<ColumnName("MinTfAnchor"), LoadColumn(27)>
		Public Property MinTfAnchor As Single


		<ColumnName("MinTfTitle"), LoadColumn(28)>
		Public Property MinTfTitle As Single


		<ColumnName("MinTfUrl"), LoadColumn(29)>
		Public Property MinTfUrl As Single


		<ColumnName("MinTfWholeDocument"), LoadColumn(30)>
		Public Property MinTfWholeDocument As Single


		<ColumnName("MinTfBody"), LoadColumn(31)>
		Public Property MinTfBody As Single


		<ColumnName("MaxTfAnchor"), LoadColumn(32)>
		Public Property MaxTfAnchor As Single


		<ColumnName("MaxTfTitle"), LoadColumn(33)>
		Public Property MaxTfTitle As Single


		<ColumnName("MaxTfUrl"), LoadColumn(34)>
		Public Property MaxTfUrl As Single


		<ColumnName("MaxTfWholeDocument"), LoadColumn(35)>
		Public Property MaxTfWholeDocument As Single


		<ColumnName("MaxTfBody"), LoadColumn(36)>
		Public Property MaxTfBody As Single


		<ColumnName("MeanTfAnchor"), LoadColumn(37)>
		Public Property MeanTfAnchor As Single


		<ColumnName("MeanTfTitle"), LoadColumn(38)>
		Public Property MeanTfTitle As Single


		<ColumnName("MeanTfUrl"), LoadColumn(39)>
		Public Property MeanTfUrl As Single


		<ColumnName("MeanTfWholeDocument"), LoadColumn(40)>
		Public Property MeanTfWholeDocument As Single


		<ColumnName("MeanTfBody"), LoadColumn(41)>
		Public Property MeanTfBody As Single


		<ColumnName("VarianceTfAnchor"), LoadColumn(42)>
		Public Property VarianceTfAnchor As Single


		<ColumnName("VarianceTfTitle"), LoadColumn(43)>
		Public Property VarianceTfTitle As Single


		<ColumnName("VarianceTfUrl"), LoadColumn(44)>
		Public Property VarianceTfUrl As Single


		<ColumnName("VarianceTfWholeDocument"), LoadColumn(45)>
		Public Property VarianceTfWholeDocument As Single


		<ColumnName("VarianceTfBody"), LoadColumn(46)>
		Public Property VarianceTfBody As Single


		<ColumnName("SumStreamLengthNormalizedTfAnchor"), LoadColumn(47)>
		Public Property SumStreamLengthNormalizedTfAnchor As Single


		<ColumnName("SumStreamLengthNormalizedTfTitle"), LoadColumn(48)>
		Public Property SumStreamLengthNormalizedTfTitle As Single


		<ColumnName("SumStreamLengthNormalizedTfUrl"), LoadColumn(49)>
		Public Property SumStreamLengthNormalizedTfUrl As Single


		<ColumnName("SumStreamLengthNormalizedTfWholeDocument"), LoadColumn(50)>
		Public Property SumStreamLengthNormalizedTfWholeDocument As Single


		<ColumnName("SumStreamLengthNormalizedTfBody"), LoadColumn(51)>
		Public Property SumStreamLengthNormalizedTfBody As Single


		<ColumnName("MinStreamLengthNormalizedTfAnchor"), LoadColumn(52)>
		Public Property MinStreamLengthNormalizedTfAnchor As Single


		<ColumnName("MinStreamLengthNormalizedTfTitle"), LoadColumn(53)>
		Public Property MinStreamLengthNormalizedTfTitle As Single


		<ColumnName("MinStreamLengthNormalizedTfUrl"), LoadColumn(54)>
		Public Property MinStreamLengthNormalizedTfUrl As Single


		<ColumnName("MinStreamLengthNormalizedTfWholeDocument"), LoadColumn(55)>
		Public Property MinStreamLengthNormalizedTfWholeDocument As Single


		<ColumnName("MinStreamLengthNormalizedTfBody"), LoadColumn(56)>
		Public Property MinStreamLengthNormalizedTfBody As Single


		<ColumnName("MaxStreamLengthNormalizedTfAnchor"), LoadColumn(57)>
		Public Property MaxStreamLengthNormalizedTfAnchor As Single


		<ColumnName("MaxStreamLengthNormalizedTfTitle"), LoadColumn(58)>
		Public Property MaxStreamLengthNormalizedTfTitle As Single


		<ColumnName("MaxStreamLengthNormalizedTfUrl"), LoadColumn(59)>
		Public Property MaxStreamLengthNormalizedTfUrl As Single


		<ColumnName("MaxStreamLengthNormalizedTfWholeDocument"), LoadColumn(60)>
		Public Property MaxStreamLengthNormalizedTfWholeDocument As Single


		<ColumnName("MaxStreamLengthNormalizedTfBody"), LoadColumn(61)>
		Public Property MaxStreamLengthNormalizedTfBody As Single


		<ColumnName("MeanStreamLengthNormalizedTfAnchor"), LoadColumn(62)>
		Public Property MeanStreamLengthNormalizedTfAnchor As Single


		<ColumnName("MeanStreamLengthNormalizedTfTitle"), LoadColumn(63)>
		Public Property MeanStreamLengthNormalizedTfTitle As Single


		<ColumnName("MeanStreamLengthNormalizedTfUrl"), LoadColumn(64)>
		Public Property MeanStreamLengthNormalizedTfUrl As Single


		<ColumnName("MeanStreamLengthNormalizedTfWholeDocument"), LoadColumn(65)>
		Public Property MeanStreamLengthNormalizedTfWholeDocument As Single


		<ColumnName("MeanStreamLengthNormalizedTfBody"), LoadColumn(66)>
		Public Property MeanStreamLengthNormalizedTfBody As Single


		<ColumnName("VarianceStreamLengthNormalizedTfAnchor"), LoadColumn(67)>
		Public Property VarianceStreamLengthNormalizedTfAnchor As Single


		<ColumnName("VarianceStreamLengthNormalizedTfTitle"), LoadColumn(68)>
		Public Property VarianceStreamLengthNormalizedTfTitle As Single


		<ColumnName("VarianceStreamLengthNormalizedTfUrl"), LoadColumn(69)>
		Public Property VarianceStreamLengthNormalizedTfUrl As Single


		<ColumnName("VarianceStreamLengthNormalizedTfWholeDocument"), LoadColumn(70)>
		Public Property VarianceStreamLengthNormalizedTfWholeDocument As Single


		<ColumnName("VarianceStreamLengthNormalizedTfBody"), LoadColumn(71)>
		Public Property VarianceStreamLengthNormalizedTfBody As Single


		<ColumnName("SumTfidfAnchor"), LoadColumn(72)>
		Public Property SumTfidfAnchor As Single


		<ColumnName("SumTfidfTitle"), LoadColumn(73)>
		Public Property SumTfidfTitle As Single


		<ColumnName("SumTfidfUrl"), LoadColumn(74)>
		Public Property SumTfidfUrl As Single


		<ColumnName("SumTfidfWholeDocument"), LoadColumn(75)>
		Public Property SumTfidfWholeDocument As Single


		<ColumnName("SumTfidfBody"), LoadColumn(76)>
		Public Property SumTfidfBody As Single


		<ColumnName("MinTfidfAnchor"), LoadColumn(77)>
		Public Property MinTfidfAnchor As Single


		<ColumnName("MinTfidfTitle"), LoadColumn(78)>
		Public Property MinTfidfTitle As Single


		<ColumnName("MinTfidfUrl"), LoadColumn(79)>
		Public Property MinTfidfUrl As Single


		<ColumnName("MinTfidfWholeDocument"), LoadColumn(80)>
		Public Property MinTfidfWholeDocument As Single


		<ColumnName("MinTfidfBody"), LoadColumn(81)>
		Public Property MinTfidfBody As Single


		<ColumnName("MaxTfidfAnchor"), LoadColumn(82)>
		Public Property MaxTfidfAnchor As Single


		<ColumnName("MaxTfidfTitle"), LoadColumn(83)>
		Public Property MaxTfidfTitle As Single


		<ColumnName("MaxTfidfUrl"), LoadColumn(84)>
		Public Property MaxTfidfUrl As Single


		<ColumnName("MaxTfidfWholeDocument"), LoadColumn(85)>
		Public Property MaxTfidfWholeDocument As Single


		<ColumnName("MaxTfidfBody"), LoadColumn(86)>
		Public Property MaxTfidfBody As Single


		<ColumnName("MeanTfidfAnchor"), LoadColumn(87)>
		Public Property MeanTfidfAnchor As Single


		<ColumnName("MeanTfidfTitle"), LoadColumn(88)>
		Public Property MeanTfidfTitle As Single


		<ColumnName("MeanTfidfUrl"), LoadColumn(89)>
		Public Property MeanTfidfUrl As Single


		<ColumnName("MeanTfidfWholeDocument"), LoadColumn(90)>
		Public Property MeanTfidfWholeDocument As Single


		<ColumnName("MeanTfidfBody"), LoadColumn(91)>
		Public Property MeanTfidfBody As Single


		<ColumnName("VarianceTfidfAnchor"), LoadColumn(92)>
		Public Property VarianceTfidfAnchor As Single


		<ColumnName("VarianceTfidfTitle"), LoadColumn(93)>
		Public Property VarianceTfidfTitle As Single


		<ColumnName("VarianceTfidfUrl"), LoadColumn(94)>
		Public Property VarianceTfidfUrl As Single


		<ColumnName("VarianceTfidfWholeDocument"), LoadColumn(95)>
		Public Property VarianceTfidfWholeDocument As Single


		<ColumnName("VarianceTfidfBody"), LoadColumn(96)>
		Public Property VarianceTfidfBody As Single


		<ColumnName("BooleanModelAnchor"), LoadColumn(97)>
		Public Property BooleanModelAnchor As Single


		<ColumnName("BooleanModelTitle"), LoadColumn(98)>
		Public Property BooleanModelTitle As Single


		<ColumnName("BooleanModelUrl"), LoadColumn(99)>
		Public Property BooleanModelUrl As Single


		<ColumnName("BooleanModelWholeDocument"), LoadColumn(100)>
		Public Property BooleanModelWholeDocument As Single


		<ColumnName("BooleanModelBody"), LoadColumn(101)>
		Public Property BooleanModelBody As Single


		<ColumnName("VectorSpaceModelAnchor"), LoadColumn(102)>
		Public Property VectorSpaceModelAnchor As Single


		<ColumnName("VectorSpaceModelTitle"), LoadColumn(103)>
		Public Property VectorSpaceModelTitle As Single


		<ColumnName("VectorSpaceModelUrl"), LoadColumn(104)>
		Public Property VectorSpaceModelUrl As Single


		<ColumnName("VectorSpaceModelWholeDocument"), LoadColumn(105)>
		Public Property VectorSpaceModelWholeDocument As Single


		<ColumnName("VectorSpaceModelBody"), LoadColumn(106)>
		Public Property VectorSpaceModelBody As Single


		<ColumnName("Bm25Anchor"), LoadColumn(107)>
		Public Property Bm25Anchor As Single


		<ColumnName("Bm25Title"), LoadColumn(108)>
		Public Property Bm25Title As Single


		<ColumnName("Bm25Url"), LoadColumn(109)>
		Public Property Bm25Url As Single


		<ColumnName("Bm25WholeDocument"), LoadColumn(110)>
		Public Property Bm25WholeDocument As Single


		<ColumnName("Bm25Body"), LoadColumn(111)>
		Public Property Bm25Body As Single


		<ColumnName("LmirAbsAnchor"), LoadColumn(112)>
		Public Property LmirAbsAnchor As Single


		<ColumnName("LmirAbsTitle"), LoadColumn(113)>
		Public Property LmirAbsTitle As Single


		<ColumnName("LmirAbsUrl"), LoadColumn(114)>
		Public Property LmirAbsUrl As Single


		<ColumnName("LmirAbsWholeDocument"), LoadColumn(115)>
		Public Property LmirAbsWholeDocument As Single


		<ColumnName("LmirAbsBody"), LoadColumn(116)>
		Public Property LmirAbsBody As Single


		<ColumnName("LmirDirAnchor"), LoadColumn(117)>
		Public Property LmirDirAnchor As Single


		<ColumnName("LmirDirTitle"), LoadColumn(118)>
		Public Property LmirDirTitle As Single


		<ColumnName("LmirDirUrl"), LoadColumn(119)>
		Public Property LmirDirUrl As Single


		<ColumnName("LmirDirWholeDocument"), LoadColumn(120)>
		Public Property LmirDirWholeDocument As Single


		<ColumnName("LmirDirBody"), LoadColumn(121)>
		Public Property LmirDirBody As Single


		<ColumnName("LmirJmAnchor"), LoadColumn(122)>
		Public Property LmirJmAnchor As Single


		<ColumnName("LmirJmTitle"), LoadColumn(123)>
		Public Property LmirJmTitle As Single


		<ColumnName("LmirJmUrl"), LoadColumn(124)>
		Public Property LmirJmUrl As Single


		<ColumnName("LmirJmWholeDocument"), LoadColumn(125)>
		Public Property LmirJmWholeDocument As Single


		<ColumnName("LmirJm"), LoadColumn(126)>
		Public Property LmirJm As Single


		<ColumnName("NumberSlashInUrl"), LoadColumn(127)>
		Public Property NumberSlashInUrl As Single


		<ColumnName("LengthUrl"), LoadColumn(128)>
		Public Property LengthUrl As Single


		<ColumnName("InlinkNumber"), LoadColumn(129)>
		Public Property InlinkNumber As Single


		<ColumnName("OutlinkNumber"), LoadColumn(130)>
		Public Property OutlinkNumber As Single


		<ColumnName("PageRank"), LoadColumn(131)>
		Public Property PageRank As Single


		<ColumnName("SiteRank"), LoadColumn(132)>
		Public Property SiteRank As Single


		<ColumnName("QualityScore"), LoadColumn(133)>
		Public Property QualityScore As Single


		<ColumnName("QualityScore2"), LoadColumn(134)>
		Public Property QualityScore2 As Single


		<ColumnName("QueryUrlClickCount"), LoadColumn(135)>
		Public Property QueryUrlClickCount As Single


		<ColumnName("UrlClickCount"), LoadColumn(136)>
		Public Property UrlClickCount As Single


		<ColumnName("UrlDwellTime"), LoadColumn(137)>
		Public Property UrlDwellTime As Single
	End Class
End Namespace
