using System.Text.Json;

using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;

using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

using Syncfusion.Blazor.DocumentEditor;
using Syncfusion.Blazor.Popups;
using static ISNAPOO.WebSystem.Pages.Candidate.CIPO.CIPOFormApplicationModal;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOFormApplicationModal : BlazorBaseComponent
    {
        [Parameter]
        public bool DisableAllFields { get; set; }

        #region Used Classes
        public class CIPOTableTrainer
        {
            public string Name { get; set; } = "";
            public string Position { get; set; } = "";
            public string Education { get; set; } = "";
            public string Experience { get; set; } = "";
        }

        private class CIPOConsultingTable
        {
            public bool InformingAndSelfInforming { get; set; } = false;
            public bool CareerCounselling { get; set; } = false;
            public bool CaseEvaluation { get; set; } = false;
            public bool PsychologicalSupport { get; set; } = false;
            public bool ActivationAndMotivation { get; set;} = false;
            public bool Advocacy { get; set; } = false;
            public bool MutualAssistanceGroups { get; set; } = false;
            public bool TalentManagement { get; set; } = false;
            public bool Mentoring { get; set; } = false;
            public bool others { get; set; } = false;
         }

        #endregion
        #region Parameters and DI
        [Parameter]
        public int IdCandidateProvider { get; set; }

        [Parameter]
        public EventCallback<bool> DocumentIsGenerated { get; set; }

        SfDocumentEditorContainer container;

        CandidateProviderVM CandidateProviderVM { get; set; }
        internal string DocumentName { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public IProfessionalDirectionService ProfessionalDirectionService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ISettingService SettingService { get; set; }

        [Inject]
        public ITemplateDocumentService TemplateDocumentService { get; set; }
        [Inject]
        public IUploadFileService uploadFileService { get; set; }

        string settingsPath;
        IEnumerable<CandidateProviderTrainerVM> trainers { get; set; }
        CIPOConsultingTable cIPOConsulting = new CIPOConsultingTable();

        private ToastMsg toast = new ToastMsg();
        SfDialog sfDialog = new SfDialog();
        private bool errorDialogVis { get; set; } = false;
        string settingResource;
        KeyValueVM kvTheory;
        KeyValueVM kvPractice;
        KeyValueVM kvPracticeAndTheory;
        List<KeyValueVM> kvConsulting; 
        string FileTemplateFilePath = "";
        //File name:Form_CIPO-JV-14.06.docx
        #endregion

        #region On Initialize
        protected override async Task OnInitializedAsync()
        {
            this.DocVis = false;
            this.isVisible = false;


        }
        #endregion

        public async Task OpenModal()
        {
            this.SpinnerShow();
            this.SpinnerHide();
            this.isVisible = true;
            this.StateHasChanged();
            await LoadInfo();
            container = new SfDocumentEditorContainer();
            this.DocVis = true;
        }

        public async Task LoadInfo()
        {
            this.SpinnerShow();

            CandidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(new CandidateProviderVM { IdCandidate_Provider = IdCandidateProvider });
            settingsPath = (await this.SettingService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            settingResource = (await this.SettingService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            Directory.CreateDirectory(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\");
            Directory.CreateDirectory(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Temp");
            this.trainers = this.CandidateProviderService.GetCandidateProviderTrainersByCandidateProviderId(this.CandidateProviderVM);
            var temp = await this.CandidateProviderService.GetAllCandidateProviderConsultingsByIdCandidateProviderAsync(CandidateProviderVM.IdCandidate_Provider);
            kvConsulting = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType", false)).ToList();
            foreach (var consult in temp)
            {
                if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "InformingAndSelfInforming")
                {
                    cIPOConsulting.InformingAndSelfInforming = true;
                }
                else if(this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "CareerCounselling")
                {
                    cIPOConsulting.CareerCounselling = true;
                }
                else if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "CaseEvaluation")
                {
                    cIPOConsulting.CaseEvaluation = true;
                }
                else if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "ActivationAndMotivation")
                {
                    cIPOConsulting.ActivationAndMotivation = true;
                }
                else if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "PsychologicalSupport")
                {
                    cIPOConsulting.PsychologicalSupport = true;
                }
                else if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "Advocacy")
                {
                    cIPOConsulting.Advocacy = true;
                }
                else if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "MutualAssistanceGroups")
                {
                    cIPOConsulting.MutualAssistanceGroups = true;
                }
                else if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "TalentManagement")
                {
                    cIPOConsulting.TalentManagement = true;

                }
                else if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "Mentoring")
                {
                    cIPOConsulting.Mentoring = true;
                }
            }
            kvTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            kvPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            kvPracticeAndTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
            FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPOApplication1")).Result.IdKeyValue).TemplatePath;
            this.SpinnerHide();
        }
        private void OnLoad()
        {
            string candidateFilePath = settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Form_CIPO-JV-14.06.docx";
            string candidateFolderPath = settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Form_CIPO-JV-14.06";
            string templateFilePath = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;
            List<CIPOConsultingTable> cIPOConsultingTables = new List<CIPOConsultingTable>();

            List<CIPOTableTrainer> cIPOTableTrainers = new List<CIPOTableTrainer>();
            if (this.trainers != null && this.trainers.Count() != 0)
            {
                foreach (var Trainer in this.trainers)
                {
                    var temp = new CIPOTableTrainer()
                    {
                        Name = Trainer.FullName,
                    };
                    if (Trainer.IdEducation != null && Trainer.IdEducation != 0)
                    {
                        try
                        {
                            temp.Education = this.DataSourceService.GetKeyValueByIdAsync(Trainer.IdEducation).Result is null ? " " : this.DataSourceService.GetKeyValueByIdAsync(Trainer.IdEducation).Result.Name;
                        }
                        catch
                        {
                            temp.Education = " ";
                        }
                    }
                    else
                    {
                        temp.Education = " ";
                    }
                    cIPOTableTrainers.Add(temp);
                }
                #region Generating file if already exists
                if (File.Exists(candidateFilePath))
                {
                    Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                    FileStream fileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    Syncfusion.DocIO.DLS.WordDocument Tempdocument = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    fileStream.Close();
                    foreach (Syncfusion.DocIO.DLS.IWTable a in Tempdocument.Sections[0].Tables)
                    {
                        if (a.Rows[0].Cells.Count == 4)
                        {
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Предишен опит в областта"))
                            {
                                RowModel = a.Rows[1];
                                foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                                {
                                    for (int i = 0; i < item.Paragraphs.Count; i++)
                                    {
                                        item.Paragraphs.RemoveAt(i);
                                    }
                                }
                                for (int i = a.Rows.Count; i > 1; --i)
                                {
                                    a.Rows.Remove(a.Rows[i - 1]);
                                }
                                a.Rows.Add(RowModel);
                                for (int j = 1; j < cIPOTableTrainers.Count(); j++)
                                {
                                    a.AddRow(isCopyFormat: true);
                                }
                            }
                        }
                        if (a.Rows[0].Cells.Count == 4)
                        {
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Предишен опит в областта"))
                            {
                                for (int i = 1; i < a.Rows.Count; i++)
                                {
                                    var temp = cIPOTableTrainers[i - 1];
                                    if (a.Rows[i].Cells[0].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                    }
                                    if (a.Rows[i].Cells[1].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                    }
                                    if (a.Rows[i].Cells[2].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                    }
                                    if (a.Rows[i].Cells[3].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                    }
                                    a.Rows[i].Cells[0].AddParagraph().AppendText(temp.Name);
                                    a.Rows[i].Cells[1].AddParagraph().AppendText(temp.Position);
                                    a.Rows[i].Cells[2].AddParagraph().AppendText(temp.Education);
                                    a.Rows[i].Cells[3].AddParagraph().AppendText(temp.Experience);

                                }

                            }
                        }
                        if (a.Rows[0].Cells.Count == 3)
                        {
                            if (a.Rows[0].Cells[0].LastParagraph.Text.Contains("Вид дейност"))
                            {
                                for (int i = 2; i < a.Rows.Count; i++)
                                {
                                    if (i != 5)
                                    {
                                        if (a.Rows[i].Cells[1].Paragraphs.Count > 0)
                                        {
                                            a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                        }
                                        if (a.Rows[i].Cells[2].Paragraphs.Count > 0)
                                        {
                                            a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                        }
                                    }
                                }


                                var a1 = a.Rows[2].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.InformingAndSelfInforming;
                                var a2 = a.Rows[2].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.InformingAndSelfInforming;
                                
                                var b = a.Rows[3].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.CareerCounselling;
                                var b1 = a.Rows[3].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.CareerCounselling;
                                
                                var c = a.Rows[4].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.CaseEvaluation;
                                var c1 = a.Rows[4].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.CaseEvaluation;

                                var d = a.Rows[6].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.PsychologicalSupport;
                                var d1 = a.Rows[6].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.PsychologicalSupport;

                                var e = a.Rows[7].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.ActivationAndMotivation;
                                var e1 = a.Rows[7].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.ActivationAndMotivation;

                                var f = a.Rows[8].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.Advocacy;
                                var f1 = a.Rows[8].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.Advocacy;

                                var g = a.Rows[9].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.MutualAssistanceGroups;
                                var g1 = a.Rows[9].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.MutualAssistanceGroups;

                                var h = a.Rows[10].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.TalentManagement;
                                var h1 = a.Rows[10].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.TalentManagement;

                                var k = a.Rows[11].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.Mentoring;
                                var k1 = a.Rows[11].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.Mentoring;

                                var j = a.Rows[12].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.others;
                                var j1 = a.Rows[12].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.others;
                            }
                        }
                    }
                        fileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    Tempdocument.Save(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    Tempdocument.Close();
                    fileStream.Close();
                }
                #endregion
                #region Generating file if it doesnt exist
                else
                {
                    Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                    FileStream fileStream = new FileStream(templateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    Syncfusion.DocIO.DLS.WordDocument Tempdocument = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    fileStream.Close();
                    foreach (Syncfusion.DocIO.DLS.IWTable a in Tempdocument.Sections[0].Tables)
                    {
                        if (a.Rows[0].Cells.Count == 4)
                        {
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Предишен опит в областта"))
                            {
                                RowModel = a.Rows[1];
                                foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                                {
                                    for (int i = 0; i < item.Paragraphs.Count; i++)
                                    {
                                        item.Paragraphs.RemoveAt(i);
                                    }
                                }
                                for (int i = a.Rows.Count; i > 1; --i)
                                {
                                    a.Rows.Remove(a.Rows[i - 1]);
                                }
                                a.Rows.Add(RowModel);
                                for (int j = 1; j < cIPOTableTrainers.Count(); j++)
                                {
                                    a.AddRow(isCopyFormat: true);
                                }
                            }
                        }
                        if (a.Rows[0].Cells.Count == 4)
                        {
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Предишен опит в областта"))
                            {
                                for (int i = 1; i < a.Rows.Count; i++)
                                {
                                    var temp = cIPOTableTrainers[i - 1];
                                    if (a.Rows[i].Cells[0].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                    }
                                    if (a.Rows[i].Cells[1].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                    }
                                    if (a.Rows[i].Cells[2].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                    }
                                    if (a.Rows[i].Cells[3].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                    }
                                    a.Rows[i].Cells[0].AddParagraph().AppendText(temp.Name);
                                    a.Rows[i].Cells[1].AddParagraph().AppendText(temp.Position);
                                    a.Rows[i].Cells[2].AddParagraph().AppendText(temp.Education);
                                    a.Rows[i].Cells[3].AddParagraph().AppendText(temp.Experience);

                                }

                            }
                        }
                        if (a.Rows[0].Cells.Count == 3)
                        {
                            if (a.Rows[0].Cells[0].LastParagraph.Text.Contains("Вид дейност"))
                            {
                                for (int i = 2; i < a.Rows.Count; i++)
                                {
                                    if (i != 5)
                                    {
                                        a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                        a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                    }
                                }


                                var a1  = a.Rows[2].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.InformingAndSelfInforming;
                                var a2 = a.Rows[2].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.InformingAndSelfInforming;

                                var b  = a.Rows[3].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.CareerCounselling;
                                var b1 = a.Rows[3].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.CareerCounselling;

                                var c  = a.Rows[4].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.CaseEvaluation;
                                var c1 = a.Rows[4].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.CaseEvaluation;

                                var d  = a.Rows[6].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.PsychologicalSupport;
                                var d1 = a.Rows[6].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.PsychologicalSupport;

                                var e  = a.Rows[7].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.ActivationAndMotivation;
                                var e1 = a.Rows[7].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.ActivationAndMotivation;

                                var f  = a.Rows[8].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.Advocacy;
                                var f1 = a.Rows[8].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.Advocacy;

                                var g  = a.Rows[9].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.MutualAssistanceGroups;
                                var g1 = a.Rows[9].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.MutualAssistanceGroups;

                                var h  = a.Rows[10].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.TalentManagement;
                                var h1 = a.Rows[10].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.TalentManagement;

                                var k  = a.Rows[11].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.Mentoring;
                                var k1 = a.Rows[11].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.Mentoring;

                                var j  = a.Rows[12].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.others;
                                var j1 = a.Rows[12].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.others;
                            }
                        }
                    }

                    fileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    Tempdocument.Save(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    Tempdocument.Close();
                    fileStream.Close();
                }
                #endregion  
                #region Reading the file and dipsplaying it on the page
                if (trainers != null && trainers.Count() != 0)
                {

                    // //Reading document to present in the page
                    FileStream fileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    Syncfusion.Blazor.DocumentEditor.WordDocument document = Syncfusion.Blazor.DocumentEditor.WordDocument.Load(fileStream, ImportFormatType.Docx);
                    fileStream.Close();
                    string json = JsonSerializer.Serialize(document);
                    document = null;
                    var dir = Directory.GetFiles(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Temp");
                    foreach (var item in dir)
                    {
                        File.Delete(item);
                    }
                    //To observe the memory go down, null out the reference of document variable.
                    SfDocumentEditor editor = container.DocumentEditor;
                    editor.OpenAsync(json);
                    //To observe the memory go down, null out the reference of json variable.
                    json = null;

                }
                #endregion
            }
        }

        #region Export 
        public void RequestExport(object args)
        {
            SfDocumentEditor documentEditor = container.DocumentEditor;
            documentEditor.SaveAsync(DocumentName, FormatType.Docx);
        }
        #endregion
        #region Saving edited document
        public async void OnProviderSave()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }
                this.SpinnerShow();
            try
            {
                SfDocumentEditor editor = container.DocumentEditor;
            byte[] data = Convert.FromBase64String(await editor.SaveAsBlobAsync(FormatType.Docx));
            Stream stream = new MemoryStream(data);
            data = null;
            using (var fileStream = new FileStream(settingsPath + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Form_CIPO-JV-14.06.docx", FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
                fileStream.Close();
            }
            stream.Close();
            stream = null;


                this.ShowSuccessAsync("Записът е успешен!");
            }
            catch (Exception)
            {

                this.ShowErrorAsync("Записът е неуспешен!");
            }
                this.SpinnerHide();
        }
        #endregion
        #region On Document Change
        public void OnDocumentChange()
        {
            string name = container.DocumentEditor.DocumentName;
            if (name != "")
            {
                DocumentName = name;
            }
        }
        #endregion
        #region Refresh Method
        public async Task OnRefresh()
        {
            DocVis = false;
            OnLoad();
            container = new SfDocumentEditorContainer();
            container.DocumentEditor = new SfDocumentEditor();
            DocVis = true;
        }
        private bool DocVis { get; set; } = false;
        #endregion
        private void OnClose()
        {
            this.SpinnerShow();
            if (this.container != null)
            {
                this.container.Dispose();
            }
            DocumentIsGenerated.InvokeAsync(false);
            this.SpinnerHide();
            this.isVisible = false;
            this.DocVis = false;
            this.StateHasChanged();
        }

        #region Is DocumentFilled
        string msg = @"Непопълнени точки: ";
        public async Task<bool> IsDocumentFilled(int CandidateProviderID)
        {
            msg = @"Непопълнени точки: ";
            bool IsFinished = false;
            settingResource = (await this.SettingService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            string candidateFilePath = settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderID}\Form_CIPO-JV-14.06.docx";
            if (File.Exists(candidateFilePath))
            {
                FileStream ValidateDocumetnFileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                Syncfusion.DocIO.DLS.WordDocument ValidateTempdocument = new Syncfusion.DocIO.DLS.WordDocument(ValidateDocumetnFileStream, Syncfusion.DocIO.FormatType.Docx);
                ValidateDocumetnFileStream.Close();

                for (int i = 0; i < ValidateTempdocument.Sections[0].Tables.Count; i++)
                {
                    Syncfusion.DocIO.DLS.IWTable table = ValidateTempdocument.Sections[0].Tables[i];
                    if (table.Rows[0].Cells.Count == 1 && table.Title != "exclude")
                    {
                        foreach (Syncfusion.DocIO.DLS.WTableRow row in table.Rows)
                        {
                            foreach (Syncfusion.DocIO.DLS.WTableCell cell in row.Cells)
                            {
                                if (cell.Paragraphs.Count != 0)
                                {
                                    var temp = "";
                                    foreach (Syncfusion.DocIO.DLS.IWParagraph paragraph in cell.Paragraphs)
                                    {
                                        temp = temp + paragraph.Text;
                                    }
                                    var smth = String.Join("", temp);
                                    if (String.IsNullOrEmpty(smth))
                                    {
                                        switch (i)
                                        {
                                            case 0:
                                                msg += "1.1, ";
                                                break;
                                            case 1:
                                                msg += "1.2, ";
                                                break;
                                            case 3:
                                                msg += "1.4, ";
                                                break;
                                            case 4:
                                                msg += "1.5, ";
                                                break;
                                            case 5:
                                                msg += "1.6, ";
                                                break;
                                            case 6:
                                                msg += "1.7, ";
                                                break;
                                            case 7:
                                                msg += "2.1.1, ";
                                                break;
                                            case 8:
                                                msg += "2.2, ";
                                                break;
                                            case 9:
                                                msg += "2.2.1, ";
                                                break;
                                            case 11:
                                                msg += "3.1.1, ";
                                                break;
                                            case 12:
                                                msg += "3.1.2, ";
                                                break;
                                            case 13:
                                                msg += "3.1.3, ";
                                                break;
                                            case 14:
                                                msg += "3.1.4, ";
                                                break;
                                            case 15:
                                                msg += "3.1.5, ";
                                                break;
                                            case 16:
                                                msg += "3.1.6, ";
                                                break;
                                            case 17:
                                                msg += "3.1.7, ";
                                                break;
                                            case 18:
                                                msg += "3.1.8, ";
                                                break;
                                            case 19:
                                                msg += "3.2.2, ";
                                                break;
                                            case 20:
                                                msg += "4.1, ";
                                                break;
                                            case 21:
                                                msg += "5.1, ";
                                                break;
                                            case 22:
                                                msg += "5.2, ";
                                                break;
                                            case 23:
                                                msg += "5.3, ";
                                                break;

                                            default:
                                                break;
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                ValidateTempdocument.Dispose();
                if (msg != @"Непопълнени точки: ")
                {
                    msg = msg.Remove(msg.Length - 2).TrimEnd().ToString() + "!";
                    return false;
                }
                return true;
            }
            else
            {
                msg = "Файлът не съществува!";
                return false;
            }
        }
        public async Task<string> MissingFieldsDocumentFilled()
        {
            return msg;
        }
        #endregion

        public async Task GetFile()
        {
            await this.LoadInfo();
            string candidateFilePath = settingResource + @$"\UploadedFiles\CandidateProvider\{this.CandidateProviderVM.IdCandidate_Provider.ToString()}\Form_CIPO-JV-14.06.docx";
            MemoryStream memoryStream = new MemoryStream();
            #region Getting Information       
            List<CIPOConsultingTable> cIPOConsultingTables = new List<CIPOConsultingTable>();

            List<CIPOTableTrainer> cIPOTableTrainers = new List<CIPOTableTrainer>();
                foreach (var Trainer in this.trainers)
                {
                    var temp = new CIPOTableTrainer()
                    {
                        Name = Trainer.FullName,
                    };
                    if (Trainer.IdEducation != null && Trainer.IdEducation != 0)
                    {
                        try
                        {
                            temp.Education = this.DataSourceService.GetKeyValueByIdAsync(Trainer.IdEducation).Result is null ? " " : this.DataSourceService.GetKeyValueByIdAsync(Trainer.IdEducation).Result.Name;
                        }
                        catch
                        {
                            temp.Education = " ";
                        }
                    }
                    else
                    {
                        temp.Education = " ";
                    }
                    cIPOTableTrainers.Add(temp);
                }
                #endregion
            if (File.Exists(candidateFilePath))
            {
                Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                FileStream fileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                Syncfusion.DocIO.DLS.WordDocument Tempdocument = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                fileStream.Close();
                foreach (Syncfusion.DocIO.DLS.IWTable a in Tempdocument.Sections[0].Tables)
                {
                    if (a.Rows[0].Cells.Count == 4)
                    {
                        if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Предишен опит в областта"))
                        {
                            RowModel = a.Rows[1];
                            foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                            {
                                for (int i = 0; i < item.Paragraphs.Count; i++)
                                {
                                    item.Paragraphs.RemoveAt(i);
                                }
                            }
                            for (int i = a.Rows.Count; i > 1; --i)
                            {
                                a.Rows.Remove(a.Rows[i - 1]);
                            }
                            a.Rows.Add(RowModel);
                            for (int j = 1; j < cIPOTableTrainers.Count(); j++)
                            {
                                a.AddRow(isCopyFormat: true);
                            }
                        }
                    }
                    if (a.Rows[0].Cells.Count == 4)
                    {
                        if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Предишен опит в областта"))
                        {
                            for (int i = 1; i < a.Rows.Count; i++)
                            {
                                var temp = cIPOTableTrainers[i - 1];
                                if (a.Rows[i].Cells[0].Paragraphs.Count > 0)
                                {
                                    a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                }
                                if (a.Rows[i].Cells[1].Paragraphs.Count > 0)
                                {
                                    a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                }
                                if (a.Rows[i].Cells[2].Paragraphs.Count > 0)
                                {
                                    a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                }
                                if (a.Rows[i].Cells[3].Paragraphs.Count > 0)
                                {
                                    a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                }
                                a.Rows[i].Cells[0].AddParagraph().AppendText(temp.Name);
                                a.Rows[i].Cells[1].AddParagraph().AppendText(temp.Position);
                                a.Rows[i].Cells[2].AddParagraph().AppendText(temp.Education);
                                a.Rows[i].Cells[3].AddParagraph().AppendText(temp.Experience);

                            }

                        }
                    }
                    if (a.Rows[0].Cells.Count == 3)
                    {
                        if (a.Rows[0].Cells[0].LastParagraph.Text.Contains("Вид дейност"))
                        {
                            for (int i = 2; i < a.Rows.Count; i++)
                            {
                                if (i != 5)
                                {
                                    if (a.Rows[i].Cells[1].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                    }
                                    if (a.Rows[i].Cells[2].Paragraphs.Count > 0)
                                    {
                                        a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                    }
                                }
                            }


                            var a1 = a.Rows[2].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.InformingAndSelfInforming;
                            var a2 = a.Rows[2].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.InformingAndSelfInforming;

                            var b = a.Rows[3].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.CareerCounselling;
                            var b1 = a.Rows[3].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.CareerCounselling;

                            var c = a.Rows[4].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.CaseEvaluation;
                            var c1 = a.Rows[4].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.CaseEvaluation;

                            var d = a.Rows[6].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.PsychologicalSupport;
                            var d1 = a.Rows[6].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.PsychologicalSupport;

                            var e = a.Rows[7].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.ActivationAndMotivation;
                            var e1 = a.Rows[7].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.ActivationAndMotivation;

                            var f = a.Rows[8].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.Advocacy;
                            var f1 = a.Rows[8].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.Advocacy;

                            var g = a.Rows[9].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.MutualAssistanceGroups;
                            var g1 = a.Rows[9].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.MutualAssistanceGroups;

                            var h = a.Rows[10].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.TalentManagement;
                            var h1 = a.Rows[10].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.TalentManagement;

                            var k = a.Rows[11].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.Mentoring;
                            var k1 = a.Rows[11].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.Mentoring;

                            var j = a.Rows[12].Cells[1].AddParagraph().AppendCheckBox().Checked = cIPOConsulting.others;
                            var j1 = a.Rows[12].Cells[2].AddParagraph().AppendCheckBox().Checked = !cIPOConsulting.others;
                        }
                    }
                }           
                Tempdocument.Save(memoryStream, Syncfusion.DocIO.FormatType.Docx);
                var kvDocType = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderDocumentType", "TemplateFormularCIPO");
                await this.uploadFileService.UploadFormularCandidateProviderDocumentAsync(memoryStream, "Form_CIPO-JV-14.06.docx", "CandidateProvider", this.CandidateProviderVM.IdCandidate_Provider, kvDocType.IdKeyValue);
                memoryStream.Dispose();
                memoryStream.Close();
            }
            else
            {
                this.ShowErrorAsync("Файлът не съществува");
            }

        }
    }
}

