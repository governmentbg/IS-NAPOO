﻿using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOOpportunityAssessment : BlazorBaseComponent
    {
        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public EventCallback<ProcedureModal> CallbackRefreshDocumentsGrid { get; set; }

        [Inject]
        public IExpertService expertService { get; set; }
        [Inject]
        public IProviderService providerService { get; set; }
        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        ToastMsg toast;
        private string kvDocTypeApplication9ToolTip = "";

        private string kvDocTypeApplication10ToolTip = "";

        private string kvDocTypeApplication11ToolTip = "";

        private string kvDocTypeApplication13ToolTip = "";

        private string kvDocTypeApplication14ToolTip = "";

        private string kvDocTypeApplication15ToolTip = "";

        private string kvDocTypeApplication16ToolTip = "";

        private string kvDocTypeApplication17ToolTip = "";

        private string kvDocTypeApplication18ToolTip = "";

        private string kvDocTypeApplication19ToolTip = "";

        private string kvDocTypeApplication20ToolTip = "";

        private string kvDocTypeApplication21ToolTip = "";
        protected override void OnInitialized()
        {
            this.kvDocTypeApplication9ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application9").Result.Description;

            this.kvDocTypeApplication10ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application10").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication11ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application11").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication13ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application13").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication14ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application14").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication15ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application15").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication16ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application16").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication17ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application17").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication18ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application18").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication19ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application19").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication20ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application20").Result.Description;
                                                                                                                       
            this.kvDocTypeApplication21ToolTip = dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application21").Result.Description;
        }
        private async Task Application9()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication9 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application9");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication9.IdKeyValue))
            {
                var doc9 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication9.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc9);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 9!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application10()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication10 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application10");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication10.IdKeyValue))
            {
                var doc10 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication10.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc10);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 10!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application11()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication11 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application11");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication11.IdKeyValue))
            {
                //Вземаме данните за може да създадем по 1 запис за всеки външен експерт
                var data = await this.providerService.GetStartedProcedureByIdForGenerateDocumentAsync(this.CandidateProviderVM.IdStartedProcedure.Value);
                var externalExperts = data.ProcedureExternalExperts.Where(pe => pe.IdProfessionalDirection != null).ToList();

                foreach (var item in externalExperts)
                {
                    var doc11 = new ProcedureDocumentVM()
                    {
                        TypeLicensing = GlobalConstants.LICENSING_CIPO,
                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IsValid = true,
                        IdDocumentType = kvDocTypeApplication11.IdKeyValue,

                        IdExpert = item.IdExpert,
                    };
                    resultContext.ResultContextObject.Add(doc11);
                }

                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 11!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application13()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication13 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application13");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication13.IdKeyValue))
            {
                var doc13 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication13.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc13);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 13!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application14()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication14 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application14");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication14.IdKeyValue))
            {
                var doc14 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication14.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc14);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 14!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application15()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication15 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application15");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication15.IdKeyValue))
            {
                //Вземаме данните за може да създадем по 1 запис за всеки член на експертната комисията
                var startedProcedureVM = await this.providerService.GetStartedProcedureByIdForGenerateDocumentAsync(this.CandidateProviderVM.IdStartedProcedure.Value);

                var expertCommision = startedProcedureVM.ProcedureExpertCommissions.FirstOrDefault();
                var commisionId = GlobalConstants.INVALID_ID_ZERO;
                if (expertCommision is not null)
                {
                    commisionId = expertCommision.IdExpertCommission;
                }

                var kvRoleCommissionMember = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertRoleCommission", "Member");
                var kvStatusActive = await dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

                //Създаваме си филтър и вземаме данните
                ExpertExpertCommissionVM filterExpertCommisionVM = new ExpertExpertCommissionVM()
                {
                    IdExpertCommission = commisionId,
                    IdStatus = kvStatusActive.IdKeyValue,

                };
                var expertExpertCommissionList = await this.expertService.GetAllExpertExpertCommissionsAsync(filterExpertCommisionVM);
                var membersfExpertCommission = expertExpertCommissionList.Where(e => e.IdRole == kvRoleCommissionMember.IdKeyValue);

                foreach (var item in membersfExpertCommission)
                {
                    var doc15 = new ProcedureDocumentVM()
                    {
                        TypeLicensing = GlobalConstants.LICENSING_CIPO,
                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IsValid = true,
                        IdDocumentType = kvDocTypeApplication15.IdKeyValue,

                        IdExpert = item.IdExpert,
                    };
                    resultContext.ResultContextObject.Add(doc15);
                }

                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 15!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application16()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication16 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application16");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue))
            {
                var doc16 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication16.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc16);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 16!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application17()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication17 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application17");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication17.IdKeyValue))
            {
                var doc17 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication17.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc17);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 17!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application18()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication18 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application18");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication18.IdKeyValue))
            {
                var doc18 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication18.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc18);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 18!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application19()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication19 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application19");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication19.IdKeyValue))
            {
                var doc19 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication19.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc19);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 19!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application20()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication20 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application20");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication20.IdKeyValue))
            {
                var doc20 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication20.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc20);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 20!";
                await toast.sfErrorToast.ShowAsync();
            }

        }

        private async Task Application21()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
            resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
            var kvDocTypeApplication21 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application21");

            if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication21.IdKeyValue))
            {
                var doc21 = new ProcedureDocumentVM()
                {
                    TypeLicensing = GlobalConstants.LICENSING_CIPO,
                    IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                    IsValid = true,
                    IdDocumentType = kvDocTypeApplication21.IdKeyValue,
                };
                resultContext.ResultContextObject.Add(doc21);


                resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }

                await CallbackRefreshDocumentsGrid.InvokeAsync();
            }
            else
            {
                toast.sfErrorToast.Content = "Вече има готови документи за Приложение 21!";
                await toast.sfErrorToast.ShowAsync();
            }

        }
    }
}