using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using MedManager.Models;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;

public class OrdonnancePdfGenerateur
{
	public void GenerateOrdonnance(string filePath, Medecin medecin, Patient patient, Ordonnance ordonnance)
	{
		using (var writer = new PdfWriter(filePath))
		{
			var pdf = new PdfDocument(writer);
			var document = new Document(pdf);

			var bold = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
			var regular = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

			DeviceRgb Bleu = new DeviceRgb(52, 83, 148);
			SolidLine solidLine = new SolidLine();
			solidLine.SetColor(Bleu);
			LineSeparator lineSeparator = new LineSeparator(solidLine);
			lineSeparator.SetWidth(UnitValue.CreatePercentValue(50));

			Table table = new Table(2);
			table.SetWidth(UnitValue.CreatePercentValue(100));
			table.SetBorder(Border.NO_BORDER);

			Cell leftCell = new Cell();
			leftCell.SetBorder(Border.NO_BORDER);
			leftCell.Add(new Paragraph($"Docteur {medecin.NomComplet}")
				.SetFont(bold)
				.SetFontSize(12)
				.SetFontColor(Bleu));
			leftCell.Add(new Paragraph($"{medecin.Specialite}")
				.SetFont(regular)
				.SetFontSize(10)
				.SetFontColor(Bleu));
			leftCell.Add(lineSeparator);
			leftCell.Add(new Paragraph($"Diplômé de la faculté de {medecin.Faculte}")
				.SetItalic()
				.SetFontSize(10)
				.SetFontColor(Bleu));
			leftCell.Add(lineSeparator);

			table.AddCell(leftCell);

			Cell rightCell = new Cell();
			rightCell.SetBorder(Border.NO_BORDER);
			rightCell.Add(new Paragraph($"{medecin.Adresse}")
				.SetItalic()
				.SetFontSize(10)
				.SetTextAlignment(TextAlignment.RIGHT)
				.SetFontColor(Bleu));
			rightCell.Add(new Paragraph($"{medecin.Ville}")
				.SetItalic()
				.SetFontSize(10)
				.SetTextAlignment(TextAlignment.RIGHT)
				.SetFontColor(Bleu));

			Paragraph lineSeparatorParagraph = new Paragraph().Add(lineSeparator).SetTextAlignment(TextAlignment.RIGHT);
			rightCell.Add(lineSeparatorParagraph);
			rightCell.Add(new Paragraph($"{medecin.NumeroTel}")
				.SetItalic()
				.SetFontSize(10)
				.SetTextAlignment(TextAlignment.RIGHT)
				.SetFontColor(Bleu));

			table.AddCell(rightCell);

			document.Add(table);

			document.Add(new Paragraph("\n"));
			document.Add(new Paragraph("\n"));
			document.Add(new Paragraph("\n"));

			document.Add(new Paragraph($"À {medecin.Ville}, le {DateTime.Now:dd/MM/yyyy}")
				.SetFontSize(10)
				.SetTextAlignment(TextAlignment.RIGHT));

			document.Add(new Paragraph("\n"));

			string civilite;
			if (patient.Sexe == Sexe.Homme)
				civilite = "Monsieur";
			else if (patient.Sexe == Sexe.Femme)
				civilite = "Madame";
			else
				civilite = "";

			document.Add(new Paragraph($"{civilite} {patient.NomComplet}")
				.SetFont(bold)
				.SetFontSize(12));

			document.Add(new Paragraph($"{patient.Age} ans, {patient.Poids} kg")
				.SetFontSize(10));


			document.Add(new Paragraph("\n"));

			foreach (var medicament in ordonnance.Medicaments)
			{
				document.Add(new Paragraph($"{medicament.Nom} {medicament.Quantite} mg")
					.SetFont(bold)
					.SetFontSize(12));

				document.Add(new Paragraph($"{medicament.Posologie}")
					.SetFontSize(10));
			}
			document.Add(new Paragraph("\n"));

			document.Add(new Paragraph("Informations supplémentaires :")
				.SetFont(bold)
				.SetFontSize(12));
			if(ordonnance.InfoSupplementaire != "")
			{
				document.Add(new Paragraph($"{ordonnance.InfoSupplementaire}"));
			}

			document.Add(new Paragraph("\n\nSignature :")
				.SetTextAlignment(TextAlignment.RIGHT));

			document.Add(new Paragraph("En cas d'urgence, contactez le 15")
				.SetFontColor(Bleu)
				.SetBold()
				.SetTextAlignment(TextAlignment.JUSTIFIED)
				.SetFixedPosition(36, 55, pdf.GetDefaultPageSize().GetWidth() - 72));
			document.Add(new Paragraph("Membre d'une association de gestion agrée\n Le réglement des honoraires par chèques est accepté.")
			.SetTextAlignment(TextAlignment.JUSTIFIED)
			.SetFontColor(Bleu)
			.SetFixedPosition(36, 20, pdf.GetDefaultPageSize().GetWidth() - 72));


			document.Close();
		}
	}
}
