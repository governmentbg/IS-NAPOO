using System;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Archive;
using Syncfusion.Blazor.Grids;
using System.IO.Compression;
using Ionic.Zip;
using ZipFile = Ionic.Zip.ZipFile;
using System.IO;

namespace ISNAPOO.WebSystem.Pages.Reports
{
    public partial class ReportNsiList : BlazorBaseComponent
    {

        [Inject]
        ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public ITrainingService trainingService { get; set; }
        [Inject]
        public IPersonService personService { get; set; }
        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        [Inject]
        public IArchiveService ArchiveService { get; set; }
        [Inject]
        public IUploadFileService uploadFileService { get; set; }

        string year = string.Empty;

        SfGrid<AnnualReportNSIVM> sfGrid;
        List<AnnualReportNSIVM> reports;
        protected override async Task OnInitializedAsync()
        {
            year = DateTime.Now.Year.ToString();
            reloadData();
        }

        public void reloadData()
        {
            reports = ArchiveService.getAllAnnualReportNSI();
        }

        public async Task zipReport()
        {
            if (loading) return;

            int number;
            if (int.TryParse(year, out number) && year != string.Empty && year.Count() == 4)
            {

                try
                {
                    MemoryStream memoryZipFile;

                    var report = reports.Where(x => x.Year == Int32.Parse(year)).FirstOrDefault();

                    var Status = await dataSourceService.GetKeyValueByIntCodeAsync("NSIReportStatus", "Created");

                    if (report is null || report.IdStatus == Status.IdKeyValue)
                    {
                        loading = true;
                        MemoryStream candidates = await candidateProviderService.GenerateExcelReportForCandidateProviders(year);

                        MemoryStream courses = await trainingService.GenerateExcelReportForCoursesAndClients(year);

                        MemoryStream MTB = await candidateProviderService.GetExcelReportForCandidateProviderPremisies(year);

                        MemoryStream curriculum = await candidateProviderService.GenerateExcelReportCurriculum(year);

                        MemoryStream qualification = await candidateProviderService.GenerateExcelReportForCandidateProviderTrainerQualification(year);

                        using (var zip = new ZipFile())
                        {
                            zip.AddEntry($"svedenia_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", candidates.ToArray());

                            zip.AddEntry($"Kursove_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", courses.ToArray());

                            zip.AddEntry($"MTB_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", MTB.ToArray());

                            zip.AddEntry($"Uchebni_planove_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", curriculum.ToArray());

                            zip.AddEntry($"Kvalifikaciq_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", qualification.ToArray());

                            memoryZipFile = new MemoryStream();

                            zip.Save(memoryZipFile);

                            await FileUtils.SaveAs(this.JsRuntime, $"Spravka_{year}.zip", memoryZipFile.ToArray());
                        }

                        var annualReports = await ArchiveService.getAnnualReportNSIByYear(year);

                        AnnualReportNSIVM annualReport = new AnnualReportNSIVM();

                        if (annualReports == null)
                        {

                            annualReport.Year = int.Parse(year);


                            annualReport.IdStatus = Status.IdKeyValue;

                            annualReport.memoryZipFile = memoryZipFile;

                            annualReport.FileName = $"Spravka_{year}.zip";

                            await ArchiveService.saveAnnualReportNSIAsync(annualReport);

                            reloadData();

                            await this.sfGrid.Refresh();
                        }else
                        {
                            annualReport.Year = int.Parse(year);

                            annualReport.memoryZipFile = memoryZipFile;

                            annualReport.FileName = $"Spravka_{year}.zip";

                            await uploadFileService.UploadFileReportNSI(annualReport);
                        }
                    }else
                    {
                        await this.ShowErrorAsync($"Вече има създаден отчен за {year} година!");
                    }
                }
                finally
                {
                    loading = false;
                }
            }
            else
            {
                await this.ShowErrorAsync("Моля въведете валидна година!");
            }
        }

        public async Task submitReport()
        {
            int number;
            if (int.TryParse(year, out number) && year != string.Empty && year.Count() == 4)
            {
                if (loading) return;

                try
                {
                    var report = await ArchiveService.getAnnualReportNSIByYear(year);
                    if (report != null)
                    {
                        if (report.Name == null)
                        {
                            var person = await personService.GetPersonByIdAsync(this.UserProps.IdPerson);

                            report.Name = $"{person.FirstName} {person.FamilyName}";

                            report.SubmissionDate = DateTime.Now;

                            var Status = await dataSourceService.GetKeyValueByIntCodeAsync("NSIReportStatus", "Submitted");

                            report.IdStatus = Status.IdKeyValue;

                            await ArchiveService.UpdateAnnualReportNSI(report);

                            reloadData();

                            await this.sfGrid.Refresh();

                            await this.ShowSuccessAsync("Подадохте отчета успешно!");
                        }
                        else
                        {
                            await this.ShowErrorAsync("Вече имате подаден отчет!");
                        }
                    }
                    else
                    {
                        await this.ShowErrorAsync("Моля изтеглете отчет!");
                    }
                }
                finally
                {
                    loading = false;
                }
            }
            else
            {
                await this.ShowErrorAsync("Моля въведете валидна година!");
            }
        }

        public async Task SelectedRow(AnnualReportNSIVM annualReportNSIVM)
        {
            MemoryStream memoryZipFile = await uploadFileService.GetReportNSIZipFile(annualReportNSIVM.Year);

            await FileUtils.SaveAs(this.JsRuntime, $"Spravka_{year}.zip", memoryZipFile.ToArray());
        }
    }
}

