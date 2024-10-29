using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using MedManager.Models;

public class OrdonnancePdfGenerateur
{
	public void GenerateOrdonnance(string filePath, Medecin medecin, Patient patient, Ordonnance ordonnance)
	{
		using (PdfWriter writer = new PdfWriter(filePath))
		{
			PdfDocument pdf = new PdfDocument(writer);
			Document document = new Document(pdf);

			
			var bold = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
			var regular = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

			
			document.Add(new Paragraph($"{medecin.NomComplet}")
				.SetFont(bold)
				.SetFontSize(12));

			document.Add(new Paragraph($"{medecin.Specialite}")
				.SetFont(regular)
				.SetFontSize(10)
				.SetUnderline());

			document.Add(new Paragraph($"Diplômé de la faculté de {medecin.Faculte}")
				.SetItalic()
				.SetFontSize(10));

			document.Add(new Paragraph($"{medecin.Adresse}\n{medecin.Ville}\n{medecin.NumeroTel}")
				.SetFontSize(10));

			
			document.Add(new Paragraph("\n"));

			
			document.Add(new Paragraph($"{patient.NomComplet}")
				.SetFont(bold)
				.SetFontSize(12));

			document.Add(new Paragraph($"{patient.Adresse}\n{patient.Ville}\n{patient.NuméroSécuritéSociale}")
				.SetFontSize(10));

			
			document.Add(new Paragraph($"\n{medecin.Ville}, le {DateTime.Now:dd/MM/yyyy}")
				.SetFontSize(10)
				.SetTextAlignment(TextAlignment.RIGHT));

			
			document.Add(new Paragraph("\n"));

			foreach (var medicament in ordonnance.Medicaments)
			{
				document.Add(new Paragraph($"{medicament.Nom} {medicament.Quantite} mg")
					.SetFont(bold)
					.SetFontSize(12));

				document.Add(new Paragraph($"{medicament.Posologie}")
					.SetFontSize(10));
			}

			document.Add(new Paragraph("\n\nSignature :")
				.SetTextAlignment(TextAlignment.RIGHT));

			document.Close();
		}
	}
}
