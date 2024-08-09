using ClosedXML.Excel;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApi.Models;

public class ExcelHelper
{
    public async Task<byte[]> CreateExcelFileAsync(List<WeatherCondition> weatherConditions)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("WeatherConditions");

            // Add headers
            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "StationId";
            worksheet.Cell(1, 3).Value = "City";
            worksheet.Cell(1, 4).Value = "District";
            worksheet.Cell(1, 5).Value = "StationName";
            worksheet.Cell(1, 6).Value = "Temperature";
            worksheet.Cell(1, 7).Value = "Humidity";
            worksheet.Cell(1, 8).Value = "WindSpeed";
            worksheet.Cell(1, 9).Value = "WindDirection";

            // Add data
            for (int i = 0; i < weatherConditions.Count; i++)
            {
                var condition = weatherConditions[i];
                worksheet.Cell(i + 2, 1).Value = condition.Date;
                worksheet.Cell(i + 2, 2).Value = condition.StationId;
                worksheet.Cell(i + 2, 3).Value = condition.City;
                worksheet.Cell(i + 2, 4).Value = condition.District;
                worksheet.Cell(i + 2, 5).Value = condition.StationName;
                worksheet.Cell(i + 2, 6).Value = condition.Temperature;
                worksheet.Cell(i + 2, 7).Value = condition.Humidity;
                worksheet.Cell(i + 2, 8).Value = condition.WindSpeed;
                worksheet.Cell(i + 2, 9).Value = condition.WindDirection;
            }

            // Save to memory stream
            using (var stream = new System.IO.MemoryStream())
            {
                workbook.SaveAs(stream);
                return await Task.FromResult(stream.ToArray());
            }
        }
    }
}
