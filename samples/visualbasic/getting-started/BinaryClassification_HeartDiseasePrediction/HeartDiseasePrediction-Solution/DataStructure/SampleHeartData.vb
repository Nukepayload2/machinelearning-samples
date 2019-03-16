Imports System.Collections.Generic

Namespace HeartDiseasePredictionConsoleApp.DataStructure
	Public Class HeartSampleData
		Friend Shared ReadOnly heartDatas As New List(Of HeartData)() From {
			New HeartData() With {
				.Age = 36.0F,
				.Sex = 1.0F,
				.Cp = 4.0F,
				.TrestBps = 145.0F,
				.Chol = 210.0F,
				.Fbs = 0.0F,
				.RestEcg = 2.0F,
				.Thalac = 148.0F,
				.Exang = 1.0F,
				.OldPeak = 1.9F,
				.Slope = 2.0F,
				.Ca = 1.0F,
				.Thal = 7.0F
			},
			New HeartData() With {
				.Age = 95.0F,
				.Sex = 1.0F,
				.Cp = 4.0F,
				.TrestBps = 145.0F,
				.Chol = 210.0F,
				.Fbs = 0.0F,
				.RestEcg = 2.0F,
				.Thalac = 148.0F,
				.Exang = 1.0F,
				.OldPeak = 1.9F,
				.Slope = 2.0F,
				.Ca = 1.0F,
				.Thal = 7.0F
			},
			New HeartData() With {
				.Age = 46.0F,
				.Sex = 1.0F,
				.Cp = 4.0F,
				.TrestBps = 135.0F,
				.Chol = 192.0F,
				.Fbs = 0.0F,
				.RestEcg = 0.0F,
				.Thalac = 148.0F,
				.Exang = 0.0F,
				.OldPeak = 0.3F,
				.Slope = 2.0F,
				.Ca = 0.0F,
				.Thal = 6.0F
			},
			New HeartData() With {
				.Age = 45.0F,
				.Sex = 0.0F,
				.Cp = 1.0F,
				.TrestBps = 140.0F,
				.Chol = 221.0F,
				.Fbs = 1.0F,
				.RestEcg = 1.0F,
				.Thalac = 150.0F,
				.Exang = 0.0F,
				.OldPeak = 2.3F,
				.Slope = 3.0F,
				.Ca = 0.0F,
				.Thal = 6.0F
			},
			New HeartData() With {
				.Age = 88.0F,
				.Sex = 0.0F,
				.Cp = 1.0F,
				.TrestBps = 140.0F,
				.Chol = 221.0F,
				.Fbs = 1.0F,
				.RestEcg = 1.0F,
				.Thalac = 150.0F,
				.Exang = 0.0F,
				.OldPeak = 2.3F,
				.Slope = 3.0F,
				.Ca = 0.0F,
				.Thal = 6.0F
			}
		}

	End Class
End Namespace
