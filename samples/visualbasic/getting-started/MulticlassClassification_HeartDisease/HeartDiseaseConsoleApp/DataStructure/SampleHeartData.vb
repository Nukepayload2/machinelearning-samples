Imports System.Collections.Generic

Namespace MulticlassClassification_HeartDisease.DataStructure
	Public Class HeartSampleData
		Friend Shared ReadOnly heartDatas As New List(Of HeartData)() From {
			New HeartData() With {
				.Age = 36.0F,
				.Sex = 1.0F,
				.Cp = 4.0F,
				.TrestBps = 135.0F,
				.Chol = 321.0F,
				.Fbs = 1.0F,
				.RestEcg = 0.0F,
				.Thalac = 158.0F,
				.Exang = 0.0F,
				.OldPeak = 1.3F,
				.Slope = 0.0F,
				.Ca = 0.0F,
				.Thal = 3.0F
			},
			New HeartData() With {
				.Age = 95.0F,
				.Sex = 1.0F,
				.Cp = 4.0F,
				.TrestBps = 135.0F,
				.Chol = 321.0F,
				.Fbs = 1.0F,
				.RestEcg = 0.0F,
				.Thalac = 158.0F,
				.Exang = 0.0F,
				.OldPeak = 1.3F,
				.Slope = 0.0F,
				.Ca = 0.0F,
				.Thal = 3.0F
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

		Friend Shared ReadOnly heart1 As New HeartData() With {
			.Age = 36.0F,
			.Sex = 1.0F,
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

		Friend Shared ReadOnly heart2 As New HeartData() With {
			.Age = 5.00F,
			.Sex = 1.0F,
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

		Friend Shared ReadOnly heart3 As New HeartData() With {
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
		}
	End Class
End Namespace
