using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;

using Data.Models.Data.Candidate;

using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;

using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DocumentEditor;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    #region Used Classes
    public class FormAppTableMTB
    {
        public string SpecialityName { get; set; } = "";
        public string ProfessionName { get; set; } = "";

        public string MDBTheoryTraining { get; set; } = "";
        public string MDBPracticalTraining { get; set; } = "";
    }
    public class FormAppTableTrainers
    {
        public string ProfessionName { get; set; } = "";
        public string SpecialityName { get; set; } = "";


        public string TrainerTheoryTraining { get; set; } = "";
        public string TrainerPracticalTraining { get; set; } = "";
    }
    #endregion
    public partial class FormApplicationModal : BlazorBaseComponent
    {
        #region Parameters and DI
        [Parameter]
        public int IdCandidateProvider { get; set; }

        [Parameter]
        public EventCallback<bool> DocumentIsGenerated { get; set; }
        [Parameter]
        public bool isCPO { get; set; }

        [Parameter]
        public bool DisableAllFields { get; set; }     

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
        #endregion

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
            public bool ActivationAndMotivation { get; set; } = false;
            public bool Advocacy { get; set; } = false;
            public bool MutualAssistanceGroups { get; set; } = false;
            public bool TalentManagement { get; set; } = false;
            public bool Mentoring { get; set; } = false;
            public bool others { get; set; } = false;
        }

        #endregion

        #region Global variables
        SfDialog sfDialog = new SfDialog();
        string settingResource;
        private bool DocVis { get; set; } = false;
        CandidateProviderVM CandidateProviderVM { get; set; }
        IEnumerable<CandidateProviderPremisesVM> premisesFromDb { get; set; }
        IEnumerable<CandidateProviderTrainerVM> trainers { get; set; }
        IEnumerable<CandidateProviderSpecialityVM> specialitiesFromDb { get; set; }
        IEnumerable<CandidateProviderPremisesVM> premises { get; set; }
        private ToastMsg toast = new ToastMsg();
        SfDocumentEditorContainer container;
        internal string DocumentName { get; set; }
        private bool errorDialogVis { get; set; } = false;

        KeyValueVM kvTheory;
        KeyValueVM kvPractice;
        KeyValueVM kvPracticeAndTheory;
        List<KeyValueVM> kvConsulting;
        CIPOConsultingTable cIPOConsulting = new CIPOConsultingTable();
        string FileTemplateFilePath = "";
        byte[] byteStream;
        #endregion

        #region On Initialize
        protected override async Task OnInitializedAsync()
        {
            this.DocVis = false;
            this.isVisible = false;
            settingResource = (await this.SettingService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
        }
        #endregion

        public async Task OpenModal()
        {
            if (isCPO)
            {
                await LoadInfo();
            }
            else
            {
                await LoadInfoCIPO();
            }
            container = new SfDocumentEditorContainer();
            this.DocVis = true;
            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task LoadInfo()
        {
            this.SpinnerShow();
            this.CandidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = this.IdCandidateProvider });
            Directory.CreateDirectory(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\");
            Directory.CreateDirectory(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Temp");
            this.premisesFromDb = await this.CandidateProviderService.GetCandidateProviderPremisesByIdAsync(this.CandidateProviderVM);
            this.specialitiesFromDb = await this.CandidateProviderService.GetCandidateProviderAllPremisesSpecialititesByCandidateProviderId(this.CandidateProviderVM);
            kvTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            kvPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            kvPracticeAndTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
            FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CPOApplication1")).Result.IdKeyValue).TemplatePath;
            this.SpinnerHide();
        }

        public async Task LoadInfoCIPO()
        {
            this.SpinnerShow();

            CandidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(new CandidateProviderVM { IdCandidate_Provider = IdCandidateProvider });
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
                else if (this.kvConsulting.First(x => x.IdKeyValue == consult.IdConsultingType).KeyValueIntCode == "CareerCounselling")
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

        #region Document Generation
        
        private void StartLoad()
        {
            if (isCPO)
            {
                OnLoad();
            }
            else
            {
                OnLoadCIPO();
            }
        }
        
        private void OnLoadCIPO()
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

        public void OnLoad()
        {
            #region Getting Information        
            string candidateFilePath = settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\FormApplication.docx";
            string candidateFolderPath = settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\FormApplication";
            string templateFilePath = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;
            if (specialitiesFromDb != null && specialitiesFromDb.Count() != 0)
            {

                List<FormAppTableMTB> formAppTableMTBs = new List<FormAppTableMTB>();
                foreach (var speciality in this.specialitiesFromDb)
                {
                    this.premises = CandidateProviderService.GetCandidateProviderPremisesBySpeciality(speciality);
                    FormAppTableMTB formAppTableMTB = new FormAppTableMTB() { ProfessionName = speciality.Speciality.Profession.CodeAndName, SpecialityName = "\n" + speciality.Speciality.CodeAndName };
                    foreach (var premis in this.premises)
                    {
                        CandidateProviderPremisesSpecialityVM spec = null;
                        if (premis.CandidateProviderPremisesSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality))
                        {
                            spec = premis.CandidateProviderPremisesSpecialities.First(x => x.IdSpeciality == speciality.IdSpeciality);
                        }
                        if (spec == null)
                        {
                            formAppTableMTB.MDBPracticalTraining += "";
                            formAppTableMTB.MDBTheoryTraining += "";
                        }
                        else
                        {
                            bool both = false;
                            bool theory = false;
                            bool practice = false;



                            both = spec.IdUsage == kvPracticeAndTheory.IdKeyValue;
                            theory = spec.IdUsage == kvTheory.IdKeyValue;
                            practice = spec.IdUsage == kvPractice.IdKeyValue;

                            if (both)
                            {
                                formAppTableMTB.MDBPracticalTraining += premis.PremisesName + ", \n";
                                formAppTableMTB.MDBTheoryTraining += premis.PremisesName + ", \n";
                            }
                            else
                            {
                                if (theory)
                                {
                                    formAppTableMTB.MDBTheoryTraining += premis.PremisesName + ", \n";
                                }
                                if (practice)
                                {
                                    formAppTableMTB.MDBPracticalTraining += premis.PremisesName + ", \n";
                                }
                            }
                        }
                    }
                    formAppTableMTBs.Add(formAppTableMTB);
                }
                this.trainers = CandidateProviderService.GetCandidateProviderTrainersByCandidateProviderId(CandidateProviderVM);
                List<FormAppTableTrainers> formAppTableTrainers = new List<FormAppTableTrainers>();
                foreach (var speciality in this.specialitiesFromDb)
                {
                    var trainersTemp = this.trainers.Where(x => x.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality));
                    FormAppTableTrainers formAppTableTrainer = new FormAppTableTrainers() { ProfessionName = speciality.Speciality.Profession.CodeAndName, SpecialityName = "\n" + speciality.Speciality.CodeAndName };
                    foreach (var trainer in trainersTemp)
                    {
                        CandidateProviderTrainerSpecialityVM spec = null;
                        if (trainer.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality))
                        {
                            spec = trainer.CandidateProviderTrainerSpecialities.First(x => x.IdSpeciality == speciality.IdSpeciality);
                        }
                        if (spec == null)
                        {
                            formAppTableTrainer.TrainerPracticalTraining += "";
                            formAppTableTrainer.TrainerTheoryTraining += "";
                        }
                        else
                        {


                            bool both = false;
                            bool theory = false;
                            bool practice = false;

                            both = spec.IdUsage == kvPracticeAndTheory.IdKeyValue;
                            theory = spec.IdUsage == kvTheory.IdKeyValue;
                            practice = spec.IdUsage == kvPractice.IdKeyValue;

                            if (both)
                            {
                                formAppTableTrainer.TrainerPracticalTraining += trainer.FullName.TrimEnd() + ", \n";
                                formAppTableTrainer.TrainerTheoryTraining += trainer.FullName.TrimEnd() + ", \n";
                            }
                            else
                            {
                                if (theory)
                                {
                                    formAppTableTrainer.TrainerTheoryTraining += trainer.FullName.TrimEnd() + ", \n";
                                }
                                if (practice)
                                {
                                    formAppTableTrainer.TrainerPracticalTraining += trainer.FullName.TrimEnd() + ", \n";
                                }
                            }
                        }
                    }
                    formAppTableTrainers.Add(formAppTableTrainer);
                }
                #endregion


                #region Generating file if it already exists
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
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("База"))
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
                                for (int j = 1; j < formAppTableMTBs.Count(); j++)
                                {
                                    a.AddRow(isCopyFormat: true);
                                }
                            }

                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Преподавател"))
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
                                for (int j = 1; j < formAppTableTrainers.Count(); j++)
                                {
                                    a.AddRow(isCopyFormat: true);
                                }
                            }
                        }
                    }
                    templateFilePath = settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Temp\FormApplication.docx";
                    fileStream = new FileStream(templateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    Tempdocument.Save(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    Tempdocument.Close();
                    Tempdocument = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    fileStream.Close();
                    foreach (Syncfusion.DocIO.DLS.IWTable a in Tempdocument.Sections[0].Tables)
                    {
                        if (a.Rows[0].Cells.Count == 4)
                        {
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("База"))
                            {
                                for (int i = 1; i < a.Rows.Count; i++)
                                {
                                    var temp = formAppTableMTBs[i - 1];
                                    a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[0].AddParagraph().AppendText(i.ToString());
                                    a.Rows[i].Cells[1].AddParagraph().AppendText(temp.ProfessionName + ", " + temp.SpecialityName);
                                    a.Rows[i].Cells[2].AddParagraph().AppendText(temp.MDBTheoryTraining);
                                    a.Rows[i].Cells[3].AddParagraph().AppendText(temp.MDBPracticalTraining);

                                }

                            }
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Преподавател"))
                            {
                                for (int i = 1; i < a.Rows.Count; i++)
                                {
                                    var temp = formAppTableTrainers[i - 1];
                                    a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[0].AddParagraph().AppendText(i.ToString());
                                    a.Rows[i].Cells[1].AddParagraph().AppendText(temp.ProfessionName + ", " + temp.SpecialityName);
                                    a.Rows[i].Cells[2].AddParagraph().AppendText(temp.TrainerTheoryTraining);
                                    a.Rows[i].Cells[3].AddParagraph().AppendText(temp.TrainerPracticalTraining);
                                }
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
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("База"))
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
                                for (int j = 1; j < formAppTableMTBs.Count(); j++)
                                {
                                    a.AddRow(isCopyFormat: true);
                                }
                            }
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Преподавател"))
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
                                for (int j = 1; j < formAppTableTrainers.Count(); j++)
                                {
                                    a.AddRow(isCopyFormat: true);
                                }
                            }
                        }
                    }
                    templateFilePath = settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Temp\FormApplication.docx";
                    fileStream = new FileStream(templateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    Tempdocument.Save(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    Tempdocument.Close();
                    Tempdocument = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    fileStream.Close();
                    foreach (Syncfusion.DocIO.DLS.IWTable a in Tempdocument.Sections[0].Tables)
                    {
                        if (a.Rows[0].Cells.Count == 4)
                        {
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("База"))
                            {
                                for (int i = 1; i < a.Rows.Count; i++)
                                {
                                    var temp = formAppTableMTBs[i - 1];
                                    a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[0].AddParagraph().AppendText(i.ToString());
                                    a.Rows[i].Cells[1].AddParagraph().AppendText(temp.ProfessionName + ", " + temp.SpecialityName);
                                    a.Rows[i].Cells[2].AddParagraph().AppendText(temp.MDBTheoryTraining);
                                    a.Rows[i].Cells[3].AddParagraph().AppendText(temp.MDBPracticalTraining);

                                }

                            }
                            if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Преподавател"))
                            {
                                for (int i = 1; i < a.Rows.Count; i++)
                                {
                                    var temp = formAppTableTrainers[i - 1];
                                    a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                    a.Rows[i].Cells[0].AddParagraph().AppendText(i.ToString());
                                    a.Rows[i].Cells[1].AddParagraph().AppendText(temp.ProfessionName + ", " + temp.SpecialityName);
                                    a.Rows[i].Cells[2].AddParagraph().AppendText(temp.TrainerTheoryTraining);
                                    a.Rows[i].Cells[3].AddParagraph().AppendText(temp.TrainerPracticalTraining);
                                }
                            }
                        }
                    }
                    fileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    Tempdocument.Save(fileStream, Syncfusion.DocIO.FormatType.Docx);
                    fileStream.Close();
                }
                #endregion
                #region Reading the file and dipsplaying it on the page
                if (specialitiesFromDb != null && specialitiesFromDb.Count() != 0)
                {
                    // //Reading document to present in the page
                    FileStream fileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                    WordDocument document = WordDocument.Load(fileStream, ImportFormatType.Docx);
                    fileStream.Close();
                    string json = JsonSerializer.Serialize(document);
                    document.Dispose();
                    document = null;
                    try
                    {
                        var dir = Directory.GetFiles(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Temp");
                        foreach (var item in dir)
                        {
                            File.Delete(item);
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    //To observe the memory go down, null out the reference of document variable.
                    SfDocumentEditor editor = container.DocumentEditor;
                    editor.Width = "100%";
                    editor.Height = "100%";
                    editor.Open(json);
                    //To observe the memory go down, null out the reference of json variable.
                    json = null;

                }
                #endregion
            }
            else
            {
                toast.sfErrorToast.Content = "Грешка при генериране на документа!\nМоля въведете информация относно специалностите на ЦПО.";
                toast.sfErrorToast.ShowAsync();
            }
            Directory.Delete(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Temp");
        }
        #endregion

        #region Export 
        public void RequestExport(object args)
        {
            SfDocumentEditor documentEditor = container.DocumentEditor;
            documentEditor.SaveAsync(DocumentName, FormatType.Docx);
        }
        #endregion

        #region Saving edited document
        
        public async Task StartSave()
        {
            if (isCPO)
            {
               await OnProviderSave();
            }
            else
            {
               await OnProviderSaveCIPO();
            }
        }

        public async Task OnProviderSave()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();
            if (isCPO)
            {
                try
                {

                    SfDocumentEditor editor = container.DocumentEditor;
                    byte[] data = Convert.FromBase64String(await editor.SaveAsBlobAsync(FormatType.Docx));
                    MemoryStream stream = new MemoryStream(data);
                    data = null;
                    using (var fileStream = new FileStream(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\FormApplication.docx", FileMode.Create, FileAccess.Write))
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
            }
            else
            {
                await OnProviderSaveCIPO();
            }
            this.SpinnerHide();

        }
        public async Task OnProviderSaveCIPO()
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
                using (var fileStream = new FileStream(settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderVM.IdCandidate_Provider}\Form_CIPO-JV-14.06.docx", FileMode.Create, FileAccess.Write))
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
            string name = "";
            if (container.DocumentEditor != null)
            {
                name = container.DocumentEditor.DocumentName;
            }
            if (name != "")
            {
                DocumentName = name;
            }
        }
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
            this.DocVis = false;
            this.isVisible = false;
            this.StateHasChanged();
        }

        #region Is DocumentFilled
        string msg = "Непопълнени точки: ";

        public async Task<bool> IsDocumentFilled(int CandidateProviderID)
        {
            if (isCPO)
            {
               return await IsDocumentFilledCPO(CandidateProviderID);
            }
            else
            {
                return await IsDocumentFilledCIPO(CandidateProviderID);
            }
        }

        public async Task<bool> IsDocumentFilledCPO(int CandidateProviderID)
        {
            msg = @"Непопълнени точки: ";
            string candidateFilePath = settingResource + @$"\UploadedFiles\CandidateProvider\{CandidateProviderID.ToString()}\FormApplication.docx";
            if (File.Exists(candidateFilePath))
            {
                FileStream ValidateDocumetnFileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                Syncfusion.DocIO.DLS.WordDocument ValidateTempdocument = new Syncfusion.DocIO.DLS.WordDocument(ValidateDocumetnFileStream, Syncfusion.DocIO.FormatType.Docx);
                ValidateDocumetnFileStream.Close();
                for (int i = 0; i < ValidateTempdocument.Sections[0].Tables.Count; i++)
                {
                    Syncfusion.DocIO.DLS.IWTable table = ValidateTempdocument.Sections[0].Tables[i];
                    if (table.Rows[0].Cells.Count != 4)
                    {
                        foreach (Syncfusion.DocIO.DLS.WTableRow row in table.Rows)
                        {
                            foreach (Syncfusion.DocIO.DLS.WTableCell cell in row.Cells)
                            {
                                if (cell.Paragraphs.Count != 0)
                                {
                                    // var collection = new List<Syncfusion.DocIO.DLS.IWParagraph>();             \
                                    var temp = "";
                                    foreach (Syncfusion.DocIO.DLS.IWParagraph paragraph in cell.Paragraphs)
                                    {
                                        temp = temp + paragraph.Text;
                                    }
                                        var smth = String.Join("", temp);
                                        if (String.IsNullOrEmpty(smth))
                                        {
                                            switch (i + 1)
                                            {
                                                case 1:
                                                    msg += "1.1.1, ";
                                                    break;
                                                case 2:
                                                    msg += "1.1.2, ";
                                                    break;
                                                case 3:
                                                    msg += "1.1.3, ";
                                                    break;
                                                case 4:
                                                    msg += "1.1.4, ";
                                                    break;
                                                case 5:
                                                    msg += "1.1.5, ";
                                                    break;
                                                case 6:
                                                    msg += "1.1.6, ";
                                                    break;
                                                case 7:
                                                    msg += "2.1.1, ";
                                                    break;
                                                case 9:
                                                    msg += "3.1.2, ";
                                                    break;
                                                case 10:
                                                    msg += "3.1.3, ";
                                                    break;
                                                case 12:
                                                    msg += "3.2.2, ";
                                                    break;
                                                case 13:
                                                    msg += "4.1.1, ";
                                                    break;
                                                case 14:
                                                    msg += "4.1.2, ";
                                                    break;
                                                case 15:
                                                    msg += "4.1.3, ";
                                                    break;
                                                case 16:
                                                    msg += "4.2.1, ";
                                                    break;
                                                case 17:
                                                    msg += "5.1, ";
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
        public async Task<bool> IsDocumentFilledCIPO(int CandidateProviderID)
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

        #region GetFile
        public async Task GetFile()
        {
            if (isCPO)
            {
                await GetFileCPO();
            }
            else
            {
                await GetFileCIPO();
            }
        }
        public async Task GetFileCPO()
        {
            await this.LoadInfo();
            string candidateFilePath = settingResource + @$"\UploadedFiles\CandidateProvider\{this.CandidateProviderVM.IdCandidate_Provider.ToString()}\FormApplication.docx";
            MemoryStream memoryStream = new MemoryStream();
            #region Getting Information        
            List<FormAppTableMTB> formAppTableMTBs = new List<FormAppTableMTB>();
            foreach (var speciality in this.specialitiesFromDb)
            {
                this.premises = CandidateProviderService.GetCandidateProviderPremisesBySpeciality(speciality);
                FormAppTableMTB formAppTableMTB = new FormAppTableMTB() { ProfessionName = speciality.Speciality.Profession.CodeAndName, SpecialityName = "\n" + speciality.Speciality.CodeAndName };
                foreach (var premis in this.premises)
                {
                    CandidateProviderPremisesSpecialityVM spec = null;
                    if (premis.CandidateProviderPremisesSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality))
                    {
                        spec = premis.CandidateProviderPremisesSpecialities.First(x => x.IdSpeciality == speciality.IdSpeciality);
                    }
                    if (spec == null)
                    {
                        formAppTableMTB.MDBPracticalTraining += "";
                        formAppTableMTB.MDBTheoryTraining += "";
                    }
                    else
                    {
                        bool both = false;
                        bool theory = false;
                        bool practice = false;



                        both = spec.IdUsage == kvPracticeAndTheory.IdKeyValue;
                        theory = spec.IdUsage == kvTheory.IdKeyValue;
                        practice = spec.IdUsage == kvPractice.IdKeyValue;

                        if (both)
                        {
                            formAppTableMTB.MDBPracticalTraining += premis.PremisesName + ", \n";
                            formAppTableMTB.MDBTheoryTraining += premis.PremisesName + ", \n";
                        }
                        else
                        {
                            if (theory)
                            {
                                formAppTableMTB.MDBTheoryTraining += premis.PremisesName + ", \n";
                            }
                            if (practice)
                            {
                                formAppTableMTB.MDBPracticalTraining += premis.PremisesName + ", \n";
                            }
                        }
                    }
                }
                formAppTableMTBs.Add(formAppTableMTB);
            }
            this.trainers = CandidateProviderService.GetCandidateProviderTrainersByCandidateProviderId(CandidateProviderVM);
            List<FormAppTableTrainers> formAppTableTrainers = new List<FormAppTableTrainers>();
            foreach (var speciality in this.specialitiesFromDb)
            {
                var trainersTemp = this.trainers.Where(x => x.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality));
                FormAppTableTrainers formAppTableTrainer = new FormAppTableTrainers() { ProfessionName = speciality.Speciality.Profession.CodeAndName, SpecialityName = "\n" + speciality.Speciality.CodeAndName };
                foreach (var trainer in trainersTemp)
                {
                    CandidateProviderTrainerSpecialityVM spec = null;
                    if (trainer.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality))
                    {
                        spec = trainer.CandidateProviderTrainerSpecialities.First(x => x.IdSpeciality == speciality.IdSpeciality);
                    }
                    if (spec == null)
                    {
                        formAppTableTrainer.TrainerPracticalTraining += "";
                        formAppTableTrainer.TrainerTheoryTraining += "";
                    }
                    else
                    {


                        bool both = false;
                        bool theory = false;
                        bool practice = false;

                        both = spec.IdUsage == kvPracticeAndTheory.IdKeyValue;
                        theory = spec.IdUsage == kvTheory.IdKeyValue;
                        practice = spec.IdUsage == kvPractice.IdKeyValue;

                        if (both)
                        {
                            formAppTableTrainer.TrainerPracticalTraining += trainer.FullName.TrimEnd() + ", \n";
                            formAppTableTrainer.TrainerTheoryTraining += trainer.FullName.TrimEnd() + ", \n";
                        }
                        else
                        {
                            if (theory)
                            {
                                formAppTableTrainer.TrainerTheoryTraining += trainer.FullName.TrimEnd() + ", \n";
                            }
                            if (practice)
                            {
                                formAppTableTrainer.TrainerPracticalTraining += trainer.FullName.TrimEnd() + ", \n";
                            }
                        }
                    }
                }
                formAppTableTrainers.Add(formAppTableTrainer);
            }
            #endregion
            if (File.Exists(candidateFilePath))
            {
                FileStream ValidateDocumetnFileStream = new FileStream(candidateFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                Syncfusion.DocIO.DLS.WordDocument Tempdocument = new Syncfusion.DocIO.DLS.WordDocument(ValidateDocumetnFileStream, Syncfusion.DocIO.FormatType.Docx);
                ValidateDocumetnFileStream.Close();
                Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                foreach (Syncfusion.DocIO.DLS.IWTable a in Tempdocument.Sections[0].Tables)
                {
                    if (a.Rows[0].Cells.Count == 4)
                    {
                        if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("База"))
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
                            for (int j = 1; j < formAppTableMTBs.Count(); j++)
                            {
                                a.AddRow(isCopyFormat: true);
                            }
                        }
                        if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Преподавател"))
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
                            for (int j = 1; j < formAppTableTrainers.Count(); j++)
                            {
                                a.AddRow(isCopyFormat: true);
                            }
                        }
                    }
                }
                Tempdocument.Save(memoryStream, Syncfusion.DocIO.FormatType.Docx);
                Tempdocument.Dispose();
                Tempdocument = new Syncfusion.DocIO.DLS.WordDocument(memoryStream, Syncfusion.DocIO.FormatType.Docx);
                foreach (Syncfusion.DocIO.DLS.IWTable a in Tempdocument.Sections[0].Tables)
                {
                    if (a.Rows[0].Cells.Count == 4)
                    {
                        if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("База"))
                        {
                            for (int i = 1; i < a.Rows.Count; i++)
                            {
                                var temp = formAppTableMTBs[i - 1];
                                a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                a.Rows[i].Cells[0].AddParagraph().AppendText(i.ToString());
                                a.Rows[i].Cells[1].AddParagraph().AppendText(temp.ProfessionName + ", " + temp.SpecialityName);
                                a.Rows[i].Cells[2].AddParagraph().AppendText(temp.MDBTheoryTraining);
                                a.Rows[i].Cells[3].AddParagraph().AppendText(temp.MDBPracticalTraining);
                            }

                        }
                        if (a.Rows[0].Cells[3].LastParagraph.Text.Contains("Преподавател"))
                        {
                            for (int i = 1; i < a.Rows.Count; i++)
                            {
                                var temp = formAppTableTrainers[i - 1];
                                a.Rows[i].Cells[0].Paragraphs.RemoveAt(0);
                                a.Rows[i].Cells[1].Paragraphs.RemoveAt(0);
                                a.Rows[i].Cells[2].Paragraphs.RemoveAt(0);
                                a.Rows[i].Cells[3].Paragraphs.RemoveAt(0);
                                a.Rows[i].Cells[0].AddParagraph().AppendText(i.ToString());
                                a.Rows[i].Cells[1].AddParagraph().AppendText(temp.ProfessionName + ", " + temp.SpecialityName);
                                a.Rows[i].Cells[2].AddParagraph().AppendText(temp.TrainerTheoryTraining);
                                a.Rows[i].Cells[3].AddParagraph().AppendText(temp.TrainerPracticalTraining);
                            }
                        }
                    }
                }
                Tempdocument.Save(memoryStream, Syncfusion.DocIO.FormatType.Docx);
                var kvDocType = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderDocumentType", "TemplateFormularCPO");
                await this.uploadFileService.UploadFormularCandidateProviderDocumentAsync(memoryStream, "FormApplication.docx", "CandidateProvider", this.CandidateProviderVM.IdCandidate_Provider, kvDocType.IdKeyValue);
                memoryStream.Dispose();
                memoryStream.Close();
            }
            else
            {
                this.ShowErrorAsync("Файлът не съществува");
            }

        }
        public async Task GetFileCIPO()
        {
            await this.LoadInfoCIPO();
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
        #endregion
    }
}
