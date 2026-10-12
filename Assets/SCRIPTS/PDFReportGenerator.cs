using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Generates a simple, real, openable PDF report of the experiment results —
/// with NO external libraries or paid plugins. It writes raw PDF syntax directly.
///
/// This is intentionally minimal (single page, plain text) so it works out of the box
/// on a Quest 2 build. If you later want tables/graphs/logos, you can swap this out
/// for a library like PdfSharp, but for a B.Tech demo this is genuinely sufficient
/// and honestly a nice thing to point at in your report/viva ("built without third-party
/// dependencies").
///
/// Usage: call PDFReportGenerator.GenerateReport(...) after a reaction completes.
/// On Quest 2 the file is saved to Application.persistentDataPath, which you can
/// pull off the headset via adb or a file browser app for the demo/viva.
/// </summary>
public static class PDFReportGenerator
{
    public static string GenerateReport(string experimentName, ReactionManager reaction, ChemicalContainer flask)
    {
        StringBuilder body = new StringBuilder();
        body.AppendLine("VR CHEMISTRY LAB - EXPERIMENT REPORT");
        body.AppendLine("--------------------------------------");
        body.AppendLine("Experiment: " + experimentName);
        body.AppendLine("Date: " + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
        body.AppendLine("");
        body.AppendLine("Result: " + reaction.lastResultSummary);
        body.AppendLine("Final pH: " + reaction.lastPH.ToString("F2"));
        body.AppendLine("Temperature: " + reaction.lastTemperatureC.ToString("F1") + " C");
        body.AppendLine("");
        body.AppendLine("Contents used:");
        foreach (var kvp in flask.contents)
        {
            body.AppendLine(" - " + kvp.Key.chemicalName + " (" + kvp.Key.formula + "): "
                + kvp.Value.ToString("F1") + " mL");
        }

        string path = Path.Combine(Application.persistentDataPath, experimentName.Replace(" ", "_") + "_Report.pdf");
        WriteSimplePdf(path, body.ToString());
        Debug.Log("PDF report saved to: " + path);
        return path;
    }

    /// <summary>
    /// Writes a minimal single-page PDF containing left-aligned text lines,
    /// using raw PDF syntax (PDF 1.4, one page, Helvetica, no images/tables).
    /// </summary>
    private static void WriteSimplePdf(string path, string content)
    {
        string[] lines = content.Replace("\r", "").Split('\n');

        StringBuilder stream = new StringBuilder();
        stream.AppendLine("BT");
        stream.AppendLine("/F1 12 Tf");
        stream.AppendLine("50 780 Td");
        stream.AppendLine("14 TL");

        foreach (string line in lines)
        {
            string safe = EscapePdfText(line);
            stream.AppendLine("(" + safe + ") Tj");
            stream.AppendLine("T*");
        }
        stream.AppendLine("ET");

        string streamContent = stream.ToString();

        using (FileStream fs = new FileStream(path, FileMode.Create))
        using (StreamWriter w = new StreamWriter(fs, Encoding.ASCII))
        {
            w.NewLine = "\n";

            System.Collections.Generic.List<long> offsets = new System.Collections.Generic.List<long>();
            void RecordOffset() { w.Flush(); offsets.Add(fs.Position); }

            w.WriteLine("%PDF-1.4");

            RecordOffset(); // obj 1
            w.WriteLine("1 0 obj");
            w.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
            w.WriteLine("endobj");

            RecordOffset(); // obj 2
            w.WriteLine("2 0 obj");
            w.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
            w.WriteLine("endobj");

            RecordOffset(); // obj 3
            w.WriteLine("3 0 obj");
            w.WriteLine("<< /Type /Page /Parent 2 0 R /Resources << /Font << /F1 5 0 R >> >> /MediaBox [0 0 612 792] /Contents 4 0 R >>");
            w.WriteLine("endobj");

            RecordOffset(); // obj 4
            w.WriteLine("4 0 obj");
            w.WriteLine("<< /Length " + Encoding.ASCII.GetByteCount(streamContent) + " >>");
            w.WriteLine("stream");
            w.Write(streamContent);
            w.WriteLine("endstream");
            w.WriteLine("endobj");

            RecordOffset(); // obj 5
            w.WriteLine("5 0 obj");
            w.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
            w.WriteLine("endobj");

            w.Flush();
            long xrefStart = fs.Position;

            w.WriteLine("xref");
            w.WriteLine("0 6");
            w.WriteLine("0000000000 65535 f ");
            foreach (long off in offsets)
                w.WriteLine(off.ToString("D10") + " 00000 n ");

            w.WriteLine("trailer");
            w.WriteLine("<< /Size 6 /Root 1 0 R >>");
            w.WriteLine("startxref");
            w.WriteLine(xrefStart.ToString());
            w.WriteLine("%%EOF");
        }
    }

    private static string EscapePdfText(string text)
    {
        return text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }
}
