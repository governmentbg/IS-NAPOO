using Data.Models.Common;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RegiX.Class.AVBulstat2.GetStateOfPlay;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.Contracts.Common;

namespace ISNAPOO.Core.Services.SPPOO
{
    public class AreaService : BaseService, IAreaService
    {
        private readonly IRepository repository;
        private readonly IProfessionalDirectionOrderService professionalDirectionOrderService;
        private readonly IProfessionOrderService professionOrderService;
        private readonly ISpecialityOrderService specialityOrderService;
        private readonly IDOCService docService;
        private readonly ISpecialityNKPDService specialityNKPDService;
        private readonly IDataSourceService dataSourceService;
        private IEnumerable<DocVM> docVMList;

        public AreaService(
            IRepository repository,
            IProfessionalDirectionOrderService professionalDirectionOrderService,
            IProfessionOrderService professionOrderService,
            ISpecialityOrderService specialityOrderService,
            IDOCService docService,
            IDataSourceService dataSourceService,
            ISpecialityNKPDService specialityNKPDService)
            : base(repository)
        {
            this.repository = repository;
            this.professionalDirectionOrderService = professionalDirectionOrderService;
            this.professionOrderService = professionOrderService;
            this.specialityOrderService = specialityOrderService;
            this.docService = docService;
            this.specialityNKPDService = specialityNKPDService;
            this.dataSourceService = dataSourceService;
            this.docVMList = new List<DocVM>();
        }

        public async Task<string> CreateAreaAsync(AreaVM areaVM)
        {
            bool checkCodeResult = this.DoesAreaWithCodeExist(areaVM.Code);

            if (checkCodeResult)
            {
                return "Област на образование с този код вече съществува!";
            }

            string msg = string.Empty;

            

            try
            {
                Area area = areaVM.To<Area>();

                await this.repository.AddAsync<Area>(area);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е успешен!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<string> DeleteAreaAsync(int id)
        {
            string msg = string.Empty;

            try
            {
                await this.repository.HardDeleteAsync<Area>(id);

                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е изтрит успешно!";
                }
                else
                {
                    msg = "Грешка при изтриване в базата данни!";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<IEnumerable<AreaVM>> GetAllAreasAsync()
        {
            IQueryable<Area> data = this.repository.All<Area>();
            var result = data.To<AreaVM>(x => x.ProfessionalDirections.Select(pr => pr.Professions.Select(s => s.Specialities)));
            return result;
        }

        public async Task<List<SPPOOTreeGridData>> LoadSPPOOData(
            List<int> areaList,
            List<int> professionalDirectionList,
            List<int> professionList,
            List<int> specialityList)
        {
            List<SPPOOTreeGridData> data = new List<SPPOOTreeGridData>();
            IEnumerable<AreaVM> areas = await this.GetAllAreasAsync();
            IEnumerable<ProfessionalDirectionOrderVM> professionalDirectionOrderVMs = await this.professionalDirectionOrderService.GetAllProfessionalDirectionOrdersAsync();
            IEnumerable<ProfessionOrderVM> professionOrderVMs = await this.professionOrderService.GetAllProfessionOrdersAsync();
            IEnumerable<SpecialityOrderVM> specialityOrderVMs = await this.specialityOrderService.GetAllSpecialityOrdersAsync();
            IEnumerable<SpecialityNKPDVM> specialityNKPDVMs = await this.specialityNKPDService.GetAllSpecialityNKPDAsync();
            this.docVMList = await this.docService.GetAllDocAsync();

            int id = 1;

            if (areaList.Count() > 0)
            {
                areas = areas.Where(a => areaList.Contains(a.IdArea)).ToList();
            }

            foreach (var area in areas)
            {
                SPPOOTreeGridData newArea = new SPPOOTreeGridData()
                {
                    Id = id++,
                    ParentId = null,
                    Name = area.Name,
                    Code = area.Code,
                    EntityId = area.IdArea,
                    EntityType = SPPOOTypes.Area,
                    IdStatus = area.IdStatus,
                    EntityParentId = null,
                    HasActiveChildren = area.ProfessionalDirections.Where(x => x.IdStatus == 15).Any(),
                    HasChildren = area.ProfessionalDirections.Any(),
                };

                data.Add(newArea);

                if (professionalDirectionList.Count() > 0)
                {
                    area.ProfessionalDirections = area.ProfessionalDirections.Where(pd => professionalDirectionList.Contains(pd.IdProfessionalDirection)).ToList();
                }

                foreach (var proffessionalDirection in area.ProfessionalDirections)
                {
                    SPPOOTreeGridData newProffessionalDirection = new SPPOOTreeGridData()
                    {
                        Id = id++,
                        ParentId = newArea.Id,
                        Name = proffessionalDirection.Name,
                        Code = proffessionalDirection.Code,
                        EntityId = proffessionalDirection.IdProfessionalDirection,
                        EntityType = SPPOOTypes.ProfessionalDirection,
                        IdStatus = proffessionalDirection.IdStatus,
                        EntityParentId = newArea.EntityId,
                        OrderNumbers = this.SetProfessionalDirectionOrderNumbers(professionalDirectionOrderVMs, proffessionalDirection.IdProfessionalDirection),
                        HasActiveChildren = proffessionalDirection.Professions.Where(x => x.IdStatus == 15).Any(),
                        HasChildren = proffessionalDirection.Professions.Any(),
                        AddOrder = this.SetProfessionalDirectionAddOrder(professionalDirectionOrderVMs, proffessionalDirection.IdProfessionalDirection, false),
                        AddOrderName = this.SetProfessionalDirectionAddOrder(professionalDirectionOrderVMs, proffessionalDirection.IdProfessionalDirection, true),
                        RemoveOrder = this.SetProfessionalDirectionRemoveOrder(professionalDirectionOrderVMs, proffessionalDirection.IdProfessionalDirection, false),
                        RemoveOrderName = this.SetProfessionalDirectionRemoveOrder(professionalDirectionOrderVMs, proffessionalDirection.IdProfessionalDirection, true),
                        ChangeOrder = this.SetProfessionalDirectionChangeOrder(professionalDirectionOrderVMs, proffessionalDirection.IdProfessionalDirection, false),
                        ChangeOrderName = this.SetProfessionalDirectionChangeOrder(professionalDirectionOrderVMs, proffessionalDirection.IdProfessionalDirection, true)
                    };

                    data.Add(newProffessionalDirection);


                    if (professionList.Count() > 0)
                    {
                        proffessionalDirection.Professions = proffessionalDirection.Professions.Where(p => professionList.Contains(p.IdProfession)).ToList();
                    }

                    foreach (var proffession in proffessionalDirection.Professions)
                    {
                        SPPOOTreeGridData newProffession = new SPPOOTreeGridData()
                        {
                            Id = id++,
                            ParentId = newProffessionalDirection.Id,
                            Name = proffession.Name,
                            Code = proffession.Code,
                            EntityId = proffession.IdProfession,
                            EntityType = SPPOOTypes.Profession,
                            IdStatus = proffession.IdStatus,
                            IsPresupposeLegalCapacity = proffession.IsPresupposeLegalCapacity,
                            EntityParentId = newProffessionalDirection.EntityId,
                            OrderNumbers = this.SetProfessionOrderNumbers(professionOrderVMs, proffession.IdProfession),
                            HasActiveChildren = proffession.Specialities.Where(x => x.IdStatus == 15).Any(),
                            HasChildren = proffession.Specialities.Any(),
                            AddOrder = this.SetProfessionAddOrder(professionOrderVMs, proffession.IdProfession, false),
                            AddOrderName = this.SetProfessionAddOrder(professionOrderVMs, proffession.IdProfession, true),
                            RemoveOrder = this.SetProfessionRemoveOrder(professionOrderVMs, proffession.IdProfession, false),
                            RemoveOrderName = this.SetProfessionRemoveOrder(professionOrderVMs, proffession.IdProfession, true),
                            ChangeOrder = this.SetProfessionChangeOrder(professionOrderVMs, proffession.IdProfession, false),
                            ChangeOrderName = this.SetProfessionChangeOrder(professionOrderVMs, proffession.IdProfession, true),
                            IdLegalCapacityOrdinanceType = proffession.IdLegalCapacityOrdinanceType.HasValue ? proffession.IdLegalCapacityOrdinanceType.Value : 0,
                        };

                        data.Add(newProffession);

                        if (specialityList.Count() > 0)
                        {
                            proffession.Specialities = proffession.Specialities.Where(s => specialityList.Contains(s.IdSpeciality)).ToList();
                        }

                        foreach (var speciality in proffession.Specialities)
                        {
                            SPPOOTreeGridData newSpeciality = new SPPOOTreeGridData()
                            {
                                Id = id++,
                                ParentId = newProffession.Id,
                                Name = speciality.Name,
                                Code = speciality.Code,
                                EntityId = speciality.IdSpeciality,
                                EntityType = SPPOOTypes.Speciality,
                                IdStatus = speciality.IdStatus,
                                IdVQS = speciality.IdVQS,
                                IdEKRLevel = speciality.IdEKRLevel,
                                IdNKRLevel = speciality.IdNKRLevel,
                                IsShortageSpecialistsLaborMarket = speciality.IsShortageSpecialistsLaborMarket,
                                IsStateProtectedSpecialties = speciality.IsStateProtectedSpecialties,
                                EntityParentId = newProffession.EntityId,
                                OrderNumbers = this.SetSpecialityOrderNumbers(specialityOrderVMs, speciality.IdSpeciality),
                                Description = this.SetDocProfessionDescription(speciality),
                                AddOrder = this.SetSpecialityAddOrder(specialityOrderVMs, speciality.IdSpeciality, false),
                                AddOrderName = this.SetSpecialityAddOrder(specialityOrderVMs, speciality.IdSpeciality, true),
                                RemoveOrder = this.SetSpecialityRemoveOrder(specialityOrderVMs, speciality.IdSpeciality, false),
                                RemoveOrderName = this.SetSpecialityRemoveOrder(specialityOrderVMs, speciality.IdSpeciality, true),
                                ChangeOrder = this.SetSpecialityChangeOrder(specialityOrderVMs, speciality.IdSpeciality, false),
                                ChangeOrderName = this.SetSpecialityChangeOrder(specialityOrderVMs, speciality.IdSpeciality, true),
                                NKPDCodes = this.SetSpecialityNKPDCodes(specialityNKPDVMs, speciality.IdSpeciality)
                            };

                            data.Add(newSpeciality);
                        }
                    }
                }
            }

            data = data.OrderBy(x => int.Parse(x.Code)).ToList();
            return data;
        }

        public async Task<AreaVM> GetAreaByIdAsync(int id)
        {
            Area area = await this.repository.GetByIdAsync<Area>(id);
            this.repository.Detach<Area>(area);

            return area.To<AreaVM>();
        }

        public async Task<string> UpdateAreaAsync(AreaVM areaVM)
        {
            Area areaFromDB = await this.repository.GetByIdAsync<Area>(areaVM.IdArea);
            this.repository.Detach<Area>(areaFromDB);

            if (areaVM.Code != areaFromDB.Code)
            {
                bool checkCodeResult = this.DoesAreaWithCodeExist(areaVM.Code);

                if (checkCodeResult)
                {
                    return "Област на образование с този код вече съществува!";
                }
            }

            string msg = string.Empty;

            try
            {
                areaFromDB = areaVM.To<Area>();
               

                this.repository.Update(areaFromDB);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е успешен!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        private bool DoesAreaWithCodeExist(string code)
        {
            return this.repository.AllReadonly<Area>(x => x.Code == code).Any();
        }

        private List<string> SetProfessionalDirectionOrderNumbers(IEnumerable<ProfessionalDirectionOrderVM> professionalDirectionOrderVMs, int id)
        {
            List<string> orderNumbers = new List<string>();

            professionalDirectionOrderVMs = professionalDirectionOrderVMs.Where(x => x.IdProfessionalDirection == id);

            foreach (var pDOrder in professionalDirectionOrderVMs)
            {
                if (pDOrder.SPPOOOrder is not null)
                {
                    orderNumbers.Add(pDOrder.SPPOOOrder.OrderNumber);
                }
            }

            return orderNumbers;
        }

        private string SetProfessionalDirectionAddOrder(IEnumerable<ProfessionalDirectionOrderVM> professionalDirectionOrderVMs, int id, bool orderNameOnly)
        {
            var addOrder = professionalDirectionOrderVMs.FirstOrDefault(x => x.IdProfessionalDirection == id && x.IdTypeChange == dataSourceService.GetOrderAddTypechangeID());

            if (addOrder != null)
            {
                if (addOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return addOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{addOrder.SPPOOOrder.OrderNumber}/{addOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private string SetProfessionalDirectionChangeOrder(IEnumerable<ProfessionalDirectionOrderVM> professionalDirectionOrderVMs, int id, bool orderNameOnly)
        {
            var changeOrder = professionalDirectionOrderVMs.FirstOrDefault(x => x.IdProfessionalDirection == id && x.IdTypeChange == dataSourceService.GetOrderChangeTypechangeID());

            if (changeOrder != null)
            {
                if (changeOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return changeOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{changeOrder.SPPOOOrder.OrderNumber}/{changeOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private string SetProfessionalDirectionRemoveOrder(IEnumerable<ProfessionalDirectionOrderVM> professionalDirectionOrderVMs, int id, bool orderNameOnly)
        {
            var addOrder = professionalDirectionOrderVMs.FirstOrDefault(x => x.IdProfessionalDirection == id && x.IdTypeChange == dataSourceService.GetOrderRemoveTypechangeID());

            if (addOrder != null)
            {
                if (addOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return addOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{addOrder.SPPOOOrder.OrderNumber}/{addOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private List<string> SetProfessionOrderNumbers(IEnumerable<ProfessionOrderVM> professionOrderVMs, int id)
        {
            List<string> orderNumbers = new List<string>();

            professionOrderVMs = professionOrderVMs.Where(x => x.IdProfession == id);

            foreach (var pOrder in professionOrderVMs)
            {
                orderNumbers.Add(pOrder.SPPOOOrder.OrderNumber);
            }

            return orderNumbers;
        }

        private string SetProfessionAddOrder(IEnumerable<ProfessionOrderVM> professionOrderVMs, int id, bool orderNameOnly)
        {
            var addOrder = professionOrderVMs.FirstOrDefault(x => x.IdProfession == id && x.IdTypeChange == dataSourceService.GetOrderAddTypechangeID());

            if (addOrder != null)
            {
                if (addOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return addOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{addOrder.SPPOOOrder.OrderNumber}/{addOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private string SetProfessionChangeOrder(IEnumerable<ProfessionOrderVM> professionOrderVMs, int id, bool orderNameOnly)
        {
            var changeOrder = professionOrderVMs.FirstOrDefault(x => x.IdProfession == id && x.IdTypeChange == dataSourceService.GetOrderChangeTypechangeID());

            if (changeOrder != null)
            {
                if (changeOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return changeOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{changeOrder.SPPOOOrder.OrderNumber}/{changeOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private string SetProfessionRemoveOrder(IEnumerable<ProfessionOrderVM> professionOrderVMs, int id, bool orderNameOnly)
        {
            var addOrder = professionOrderVMs.FirstOrDefault(x => x.IdProfession == id && x.IdTypeChange == dataSourceService.GetOrderRemoveTypechangeID());

            if (addOrder != null)
            {
                if (addOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return addOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{addOrder.SPPOOOrder.OrderNumber}/{addOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private List<string> SetSpecialityOrderNumbers(IEnumerable<SpecialityOrderVM> specialityOrderVMs, int id)
        {
            List<string> orderNumbers = new List<string>();

            specialityOrderVMs = specialityOrderVMs.Where(x => x.IdSpeciality == id);

            foreach (var sOrder in specialityOrderVMs)
            {
                orderNumbers.Add(sOrder.SPPOOOrder.OrderNumber);
            }

            return orderNumbers;
        }

        private string SetSpecialityAddOrder(IEnumerable<SpecialityOrderVM> specialityOrderVMs, int id, bool orderNameOnly)
        {
            var addOrder = specialityOrderVMs.FirstOrDefault(x => x.IdSpeciality == id && x.IdTypeChange == dataSourceService.GetOrderAddTypechangeID());

            if (addOrder != null)
            {
                if (addOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return addOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{addOrder.SPPOOOrder.OrderNumber}/{addOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private string SetSpecialityChangeOrder(IEnumerable<SpecialityOrderVM> specialityOrderVMs, int id, bool orderNameOnly)
        {
            var changeOrder = specialityOrderVMs.FirstOrDefault(x => x.IdSpeciality == id && x.IdTypeChange == dataSourceService.GetOrderChangeTypechangeID());

            if (changeOrder != null)
            {
                if (changeOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return changeOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{changeOrder.SPPOOOrder.OrderNumber}/{changeOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private string SetSpecialityRemoveOrder(IEnumerable<SpecialityOrderVM> specialityOrderVMs, int id, bool orderNameOnly)
        {
            var addOrder = specialityOrderVMs.FirstOrDefault(x => x.IdSpeciality == id && x.IdTypeChange == dataSourceService.GetOrderRemoveTypechangeID());

            if (addOrder != null)
            {
                if (addOrder.SPPOOOrder != null)
                {
                    if (orderNameOnly)
                    {
                        return addOrder.SPPOOOrder.OrderNumber;
                    }
                    else
                    {
                        return $"{addOrder.SPPOOOrder.OrderNumber}/{addOrder.SPPOOOrder.OrderDate.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }

            return string.Empty;
        }

        private string SetDocProfessionDescription(SpecialityVM speciality)
        {
            DocVM docVM = this.docVMList.FirstOrDefault(x => x.IdDOC == speciality.IdDOC);

            if (docVM != null)
            {
                speciality.Doc = docVM;

                return speciality.Doc.DescriptionProfession;
            }

            return string.Empty;
        }

        private List<string> SetSpecialityNKPDCodes(IEnumerable<SpecialityNKPDVM> specialityNKPDVMs, int id)
        {
            List<string> nkpdCodes = new List<string>();

            specialityNKPDVMs = specialityNKPDVMs.Where(x => x.IdSpeciality == id);

            foreach (var sNkpd in specialityNKPDVMs)
            {
                nkpdCodes.Add(sNkpd.NKPD.Code);
            }

            return nkpdCodes;
        }

        public MemoryStream GenerateSPPOOReport(List<SPPOOTreeGridData> SPPOOSource, IEnumerable<KeyValueVM> spkValues)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\NAPOO\SPPOO";
            FileStream template = new FileStream($@"{resources_Folder}\SPPOO.xlsx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook = application.Workbooks.Open(template, ExcelOpenType.Automatic);
            IWorksheet worksheet = workbook.Worksheets[0];

            var firstVQS = spkValues.FirstOrDefault(x => x.KeyValueIntCode == "I_VQS").IdKeyValue;
            var secondVQS = spkValues.FirstOrDefault(x => x.KeyValueIntCode == "II_VQS").IdKeyValue;
            var thirdVQS = spkValues.FirstOrDefault(x => x.KeyValueIntCode == "III_VQS").IdKeyValue;
            var fourthVQS = spkValues.FirstOrDefault(x => x.KeyValueIntCode == "IV_VQS").IdKeyValue;

            var rowCounter = 3;
            var specialities = SPPOOSource.Where(x => x.EntityType == SPPOOTypes.Speciality);
            foreach (var speciality in specialities)
            {
                var profession = SPPOOSource.FirstOrDefault(x => x.Id == speciality.ParentId);
                var professionalDirection = SPPOOSource.FirstOrDefault(x => x.Id == profession?.ParentId);
                var area = SPPOOSource.FirstOrDefault(x => x.Id == professionalDirection?.ParentId);

                worksheet.Range[$"A{rowCounter}"].Text = area?.Code;
                worksheet.Range[$"B{rowCounter}"].Text = area?.Name;
                worksheet.Range[$"C{rowCounter}"].Text = professionalDirection?.Code;
                worksheet.Range[$"D{rowCounter}"].Text = professionalDirection?.Name;
                worksheet.Range[$"E{rowCounter}"].Text = profession?.Code;
                worksheet.Range[$"F{rowCounter}"].Text = profession?.Name;
                worksheet.Range[$"G{rowCounter}"].Text = speciality.Code;
                worksheet.Range[$"H{rowCounter}"].Text = speciality.Name;

                if (speciality.IdVQS == firstVQS)
                {
                    worksheet.Range[$"I{rowCounter}"].Text = "X";
                    worksheet.Range[$"I{rowCounter}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range[$"I{rowCounter}"].CellStyle.Font.Bold = true;
                }
                else if (speciality.IdVQS == secondVQS)
                {
                    worksheet.Range[$"J{rowCounter}"].Text = "X";
                    worksheet.Range[$"J{rowCounter}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range[$"J{rowCounter}"].CellStyle.Font.Bold = true;
                }
                else if (speciality.IdVQS == thirdVQS)
                {
                    worksheet.Range[$"K{rowCounter}"].Text = "X";
                    worksheet.Range[$"K{rowCounter}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range[$"K{rowCounter}"].CellStyle.Font.Bold = true;
                }
                else
                {
                    worksheet.Range[$"L{rowCounter}"].Text = "X";
                    worksheet.Range[$"L{rowCounter}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range[$"L{rowCounter}"].CellStyle.Font.Bold = true;
                }

                if (speciality.NKPDCodes.Any())
                {
                    worksheet.Range[$"M{rowCounter}"].Text = string.Join("; ", speciality.NKPDCodes);
                }

                rowCounter++;
            }

            MemoryStream stream = new MemoryStream();

            workbook.SaveAs(stream);
            template.Dispose();
            excelEngine.Dispose();

            return stream;
        }
    }
}
