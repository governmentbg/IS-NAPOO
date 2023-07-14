using Data.Models.Common;

using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC.NKPD;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using RegiX;
using RegiX.Class.NKPD.GetNKPDAllData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static ISNAPOO.Common.Enums.EMIEnums;

namespace ISNAPOO.Core.Services.DOC.NKPD
{
    public class NKPDService : BaseService, INKPDService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly IRegiXService RegiXService;
        private readonly IKeyTypeService KeyTypeService;
        private readonly IKeyValueService KeyValueService;

        public NKPDService(IRepository repository, IDataSourceService dataSourceService, AuthenticationStateProvider authenticationStateProvider, IRegiXService RegiXService, IKeyTypeService KeyTypeService, IKeyValueService KeyValueService)
            : base(repository, authenticationStateProvider)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this.RegiXService = RegiXService;
            this.KeyTypeService = KeyTypeService;
            this.KeyValueService = KeyValueService;
        }
        ResultContext<KeyValueVM> resultContext = new ResultContext<KeyValueVM>();

        public async Task<int> CreateNKPDAsync(NKPDVM nKPDVM)
        {
            Data.Models.Data.DOC.NKPD nkpd = nKPDVM.To<Data.Models.Data.DOC.NKPD>();
            await this.repository.AddAsync<Data.Models.Data.DOC.NKPD>(nkpd);

            return await this.repository.SaveChangesAsync();
        }

        public async Task UpdateNKPD(NKPDVM nKPDVM)
        {
            var DBNKPD = await this.repository.GetByIdAsync<Data.Models.Data.DOC.NKPD>(nKPDVM.IdNKPD);

            DBNKPD.Name = nKPDVM.Name;

            repository.Update<Data.Models.Data.DOC.NKPD>(DBNKPD);
            await repository.SaveChangesAsync();
        }

        public async Task<int> DeleteNKPDAsync(int id)
        {
            Data.Models.Data.DOC.NKPD nKPD = await this.repository.GetByIdAsync<Data.Models.Data.DOC.NKPD>(id);

            if (nKPD != null)
            {
                this.repository.Detach<Data.Models.Data.DOC.NKPD>(nKPD);
                this.repository.HardDelete<Data.Models.Data.DOC.NKPD>(nKPD);
                return await this.repository.SaveChangesAsync();
            }

            return 0;
        }
        public async Task<IEnumerable<NKPDVM>> GetAllNKPDOnlyAsync()
        {
            var nKPDs = this.repository.All<Data.Models.Data.DOC.NKPD>().OrderBy(n => n.Code);
            var dataVM = nKPDs.To<NKPDVM>();
            return dataVM;
        }
        public async Task<IEnumerable<NKPDVM>> GetAllNKPDAsync()
        {
            IQueryable<Data.Models.Data.DOC.NKPD> nKPDs = this.repository.All<Data.Models.Data.DOC.NKPD>().OrderBy(n=>n.Code);

            var preorderedNKPDs = await nKPDs.To<NKPDVM>().ToListAsync();
            var keyValues = await this.KeyValueService.GetAllAsync();

            foreach (var item in preorderedNKPDs)
            {
                //Class
                if (keyValues.Where(k => item.IdClassCode == k.IdKeyValue).FirstOrDefault().Name == null 
                    && keyValues.Where(k => item.IdClassCode == k.IdKeyValue).FirstOrDefault().KeyValueIntCode == null)
                {
                    item.ClassName = " ";
                    item.ClassCode = " ";
                }
                else
                {
                    item.ClassName = keyValues.Where(k => item.IdClassCode == k.IdKeyValue).FirstOrDefault().Name;
                    item.ClassCode = keyValues.Where(k => item.IdClassCode == k.IdKeyValue).FirstOrDefault().KeyValueIntCode;
                }
                //Subclass
                if (keyValues.Where(k => item.IdSubclassCode == k.IdKeyValue).FirstOrDefault().Name == null
                    && keyValues.Where(k => item.IdSubclassCode == k.IdKeyValue).FirstOrDefault().KeyValueIntCode == null)
                {
                    item.SubclassName = " ";
                    item.SubclassCode = " ";
                }
                else
                {
                    item.SubclassName = keyValues.Where(k => item.IdSubclassCode == k.IdKeyValue).FirstOrDefault().Name;
                    item.SubclassCode = keyValues.Where(k => item.IdSubclassCode == k.IdKeyValue).FirstOrDefault().KeyValueIntCode;
                }
                //Group
                if (keyValues.Where(k => item.IdGroupCode == k.IdKeyValue).FirstOrDefault().Name == null
                    && keyValues.Where(k => item.IdGroupCode == k.IdKeyValue).FirstOrDefault().KeyValueIntCode == null)
                {
                    item.GroupName = " ";
                    item.GroupCode = " ";
                }
                else
                {
                    item.GroupName = keyValues.Where(k => item.IdGroupCode == k.IdKeyValue).FirstOrDefault().Name;
                    item.GroupCode = keyValues.Where(k => item.IdGroupCode == k.IdKeyValue).FirstOrDefault().KeyValueIntCode;
                }
                //IndividualGroup
                if (keyValues.Where(k => item.IdIndividualGroupCode == k.IdKeyValue).FirstOrDefault().Name == null
                    && keyValues.Where(k => item.IdIndividualGroupCode == k.IdKeyValue).FirstOrDefault().KeyValueIntCode == null)
                {
                    item.IndividualGroupName = " ";
                    item.IndividualGroupCode = " ";
                }
                else
                {
                    item.IndividualGroupName = keyValues.Where(k => item.IdIndividualGroupCode == k.IdKeyValue).FirstOrDefault().Name;
                    item.IndividualGroupCode = keyValues.Where(k => item.IdIndividualGroupCode == k.IdKeyValue).FirstOrDefault().KeyValueIntCode;
                }
            }
            return preorderedNKPDs.OrderBy(x => x.CodeFormattedInt).ToList();
        }

        public async Task<NKPDVM> GetNKPDByIdAsync(int id)
        {
            Data.Models.Data.DOC.NKPD nKPD = await this.repository.GetByIdAsync<Data.Models.Data.DOC.NKPD>(id);

            if (nKPD != null)
            {
                this.repository.Detach<Data.Models.Data.DOC.NKPD>(nKPD);
                return nKPD.To<NKPDVM>();
            }

            return null;
        }

        public async Task<IEnumerable<NKPDVM>> GetNKPDsByIdsAsync(List<int> ids)
        {
            IQueryable<Data.Models.Data.DOC.NKPD> nKPDs = this.repository.All<Data.Models.Data.DOC.NKPD>(this.FilterByIds(ids));

            return await nKPDs.To<NKPDVM>().ToListAsync();
        }

        public async Task<NKPDVM> GetNKPDByCodeAsync(string code)
        {
           Data.Models.Data.DOC.NKPD nKPD = await this.repository.All<Data.Models.Data.DOC.NKPD>(n => n.Code == code).FirstOrDefaultAsync();

           return nKPD.To<NKPDVM>();
        }

        public async Task<List<NKPDTreeGridData>> LoadNKPDDataAsync()
        {
            List<NKPDTreeGridData> nKPDTreeGridData = new List<NKPDTreeGridData>();

            ///НКПД - Код на клас
            var nkpdClassCodeList = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("NKPDClassCode");
            var nkpdSubclassCodeList = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("NKPDSubclassCode");
            var nkpdGroupCodeList = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("NKPDGroupCode");
            var nkpdIndividualGroupList = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("NKPDIndividualGroupCode");
            var nkpdCodeList = await this.GetAllNKPDAsync();

            int id = 1;

            foreach (var classCode in nkpdClassCodeList)
            {
                NKPDTreeGridData newClassCode = new NKPDTreeGridData()
                {
                    Id = id++,
                    ParentId = null,
                    Name = classCode.Name,
                    Code = classCode.KeyValueIntCode,
                    EntityId = classCode.IdKeyValue,
                    NKPDLevel = NKPDLevel.ClassCode,
                    EntityType = "NKPDClassCode",
                    IsParent = nkpdSubclassCodeList.Where(k => k.DefaultValue1 == classCode.KeyValueIntCode).Any()
                };

                nKPDTreeGridData.Add(newClassCode);

                foreach (var subclassCode in nkpdSubclassCodeList.Where(k => k.DefaultValue1 == classCode.KeyValueIntCode))
                {
                    NKPDTreeGridData newSubclassCode = new NKPDTreeGridData()
                    {
                        Id = id++,
                        ParentId = newClassCode.Id,
                        Name = subclassCode.Name,
                        Code = subclassCode.KeyValueIntCode,
                        EntityId = subclassCode.IdKeyValue,
                        NKPDLevel = NKPDLevel.SubclassCode,
                        EntityType = "NKPDSubclassCode",
                        IsParent = nkpdGroupCodeList.Where(k => k.DefaultValue1 == subclassCode.KeyValueIntCode).Any()
                    };

                    nKPDTreeGridData.Add(newSubclassCode);

                    foreach (var groupCode in nkpdGroupCodeList.Where(k => k.DefaultValue1 == subclassCode.KeyValueIntCode))
                    {
                        NKPDTreeGridData newGroupCode = new NKPDTreeGridData()
                        {
                            Id = id++,
                            ParentId = newSubclassCode.Id,
                            Name = groupCode.Name,
                            Code = groupCode.KeyValueIntCode,
                            EntityId = groupCode.IdKeyValue,
                            NKPDLevel = NKPDLevel.IndividualGroup,
                            EntityType = "NKPDGroupCode",
                            IsParent = nkpdIndividualGroupList.Where(k => k.DefaultValue1 == groupCode.KeyValueIntCode).Any()
                        };

                        nKPDTreeGridData.Add(newGroupCode);

                        foreach (var individualGroup in nkpdIndividualGroupList.Where(k => k.DefaultValue1 == groupCode.KeyValueIntCode))
                        {
                            NKPDTreeGridData newIndividualGroup = new NKPDTreeGridData()
                            {
                                Id = id++,
                                ParentId = newGroupCode.Id,
                                Name = individualGroup.Name,
                                Code = individualGroup.KeyValueIntCode,
                                EntityId = individualGroup.IdKeyValue,
                                NKPDLevel = NKPDLevel.IndividualGroup,
                                EntityType = "NKPDIndividualGroupCode",
                                IsParent = nkpdCodeList.Where(k =>
                                    k.IdClassCode == classCode.IdKeyValue &&
                                    k.IdSubclassCode == subclassCode.IdKeyValue &&
                                    k.IdGroupCode == groupCode.IdKeyValue &&
                                    k.IdIndividualGroupCode == individualGroup.IdKeyValue).Any()
                            };

                            nKPDTreeGridData.Add(newIndividualGroup);

                            foreach (var nkpdCode in nkpdCodeList.Where(k =>
                                k.IdClassCode == classCode.IdKeyValue &&
                                k.IdSubclassCode == subclassCode.IdKeyValue &&
                                k.IdGroupCode == groupCode.IdKeyValue &&
                                k.IdIndividualGroupCode == individualGroup.IdKeyValue
                            ))
                            {
                                NKPDTreeGridData newNkpdCode = new NKPDTreeGridData()
                                {
                                    Id = id++,
                                    ParentId = newIndividualGroup.Id,
                                    Name = nkpdCode.Name,
                                    Code = nkpdCode.Code,
                                    EntityId = nkpdCode.IdNKPD,
                                    NKPDLevel = NKPDLevel.NkpdCode,
                                    EntityType = "NKPDCode"
                                };

                                nKPDTreeGridData.Add(newNkpdCode);
                            }
                        }
                    }
                }
            }

            return nKPDTreeGridData;
        }

        protected Expression<Func<Data.Models.Data.DOC.NKPD, bool>> FilterByIds(List<int> ids)
        {
            var predicate = PredicateBuilder.True<Data.Models.Data.DOC.NKPD>();

            predicate = predicate.And(n => ids.Contains(n.IdNKPD));

            return predicate;
        }

        public async Task<List<string>> UpdateNKPDTableAsync()
        {
            List<string> updateLogger = new List<string>();
            //RegiXService.PersonInCompaniesSearch("123123123", RegiXService.GetCallContext());
            var nkpd = RegiXService.GetNKPDAllData(DateTime.Now, RegiXService.GetCallContext());


            var classTypes = nkpd.NKPD.Where(s => s.Type.ToString() == "class");
            var subClassTypes = nkpd.NKPD.Where(s => s.Type.ToString() == "subClass");
            var groupTypes = nkpd.NKPD.Where(s => s.Type.ToString() == "group");
            var individualGroupTypes = nkpd.NKPD.Where(s => s.Type.ToString() == "individualGroup");
            var nkpdTypes = nkpd.NKPD.Where(s => s.Type.ToString() == "nkpd");




            await this.InsertSubclassTypesInDb(subClassTypes, updateLogger);
            await this.InsertGroupTypesInDb(groupTypes, updateLogger);
            await this.InsertIndividualGroupTypesInDb(individualGroupTypes, updateLogger);
            await this.InsertNKPDTypesInDb(nkpdTypes, updateLogger);
            return updateLogger;

        }
        private async Task InsertSubclassTypesInDb(IEnumerable<NKPDEntry> nkpdType, List<string> updateLogger)
        {
            var keyTypes = await this.KeyTypeService.GetAllKeyTypesAsync();
            var keyValues = await this.KeyValueService.GetAllAsync();

            var nkpdKeyType = keyTypes.Where(t => t.KeyTypeIntCode == "NKPDSubclassCode").FirstOrDefault();
            var orderCounter = keyValues.Where(t => t.IdKeyType == nkpdKeyType.IdKeyType).ToList().Count() + 1;

            foreach (var nkpd in nkpdType)
            {

                var newClassType = new KeyValueVM
                {
                    IdKeyType = nkpdKeyType.IdKeyType,
                    Name = nkpd.Name,
                    KeyValueIntCode = nkpd.Code,
                    Description = nkpd.Name,
                    Order = orderCounter,
                    DefaultValue1 = nkpd.ClassCode
                };

                var nkpdSubclassTypeList = await this.KeyValueService.GetAllNKPDClassValuesViaKeyTypeIntCodeAsync(nkpdKeyType.KeyTypeIntCode);
                var isClassExisting = nkpdSubclassTypeList.Any(t => t.KeyValueIntCode == newClassType.KeyValueIntCode);

                if (!isClassExisting)
                {
                    resultContext.ResultContextObject = newClassType;
                    await this.KeyValueService.CreateKeyValueAsync(resultContext);
                    updateLogger.Add($"Нов подклас е добавен: Код - [{newClassType.KeyValueIntCode}] Име - [{newClassType.Name}]");
                }
                else
                {
                    var currentNKPD = nkpdSubclassTypeList.Where(t => t.KeyValueIntCode == newClassType.KeyValueIntCode).FirstOrDefault();
                    string oldName = currentNKPD.Name;
                    var isNKPDNameChange = (currentNKPD.Name != newClassType.Name);
                    if (isNKPDNameChange)
                    {
                        currentNKPD.Name = newClassType.Name;
                        currentNKPD.Description = newClassType.Name;
                        resultContext.ResultContextObject = currentNKPD;
                        await this.KeyValueService.UpdateKeyValueAsync(resultContext);
                        updateLogger.Add($"Името на подклас[{newClassType.KeyValueIntCode}] е променено: Старо:[{oldName}] - Ново:[{newClassType.Name}]");
                    }
                }

            }
        }
        private async Task InsertGroupTypesInDb(IEnumerable<NKPDEntry> nkpdType, List<string> updateLogger)
        {
            var keyTypes = await this.KeyTypeService.GetAllKeyTypesAsync();
            var keyValues = await this.KeyValueService.GetAllAsync();

            var nkpdKeyType = keyTypes.Where(t => t.KeyTypeIntCode == "NKPDGroupCode").FirstOrDefault();
            var orderCounter = keyValues.Where(t => t.IdKeyType == nkpdKeyType.IdKeyType).ToList().Count() + 1;

            foreach (var nkpd in nkpdType)
            {
                var newClassType = new KeyValueVM
                {
                    IdKeyType = nkpdKeyType.IdKeyType,
                    Name = nkpd.Name,
                    KeyValueIntCode = nkpd.Code,
                    Description = nkpd.Name,
                    Order = orderCounter,
                    DefaultValue1 = nkpd.SubclassCode
                };

                var nkpdGroupTypeList = await this.KeyValueService.GetAllNKPDClassValuesViaKeyTypeIntCodeAsync(nkpdKeyType.KeyTypeIntCode);
                var isClassExisting = nkpdGroupTypeList.Any(t => t.KeyValueIntCode == newClassType.KeyValueIntCode);

                if (!isClassExisting)
                {
                    resultContext.ResultContextObject = newClassType;
                    await this.KeyValueService.CreateKeyValueAsync(resultContext);
                    updateLogger.Add($"Нова група е добавена: Код - [{newClassType.KeyValueIntCode}] Име - [{newClassType.Name}]");
                }
                else
                {
                    var currentNKPD = nkpdGroupTypeList.Where(t => t.KeyValueIntCode == newClassType.KeyValueIntCode).FirstOrDefault();
                    string oldName = currentNKPD.Name;
                    var isNKPDNameChange = (currentNKPD.Name != newClassType.Name);
                    if (isNKPDNameChange)
                    {
                        currentNKPD.Name = newClassType.Name;
                        currentNKPD.Description = newClassType.Name;
                        resultContext.ResultContextObject = currentNKPD;
                        await this.KeyValueService.UpdateKeyValueAsync(resultContext);
                        updateLogger.Add($"Името на групата[{newClassType.KeyValueIntCode}] е променено: Старо:[{oldName}] - Ново:[{newClassType.Name}]");
                    }
                }
            }
        }
        private async Task InsertIndividualGroupTypesInDb(IEnumerable<NKPDEntry> nkpdType, List<string> updateLogger)
        {
            var keyTypes = await this.KeyTypeService.GetAllKeyTypesAsync();
            var keyValues = await this.KeyValueService.GetAllAsync();

            var nkpdKeyType = keyTypes.Where(t => t.KeyTypeIntCode == "NKPDIndividualGroupCode").FirstOrDefault();
            var orderCounter = keyValues.Where(t => t.IdKeyType == nkpdKeyType.IdKeyType).ToList().Count() + 1;

            foreach (var nkpd in nkpdType)
            {
                var newClassType = new KeyValueVM
                {
                    IdKeyType = nkpdKeyType.IdKeyType,
                    Name = nkpd.Name,
                    KeyValueIntCode = nkpd.Code,
                    Description = nkpd.Name,
                    Order = orderCounter,
                    DefaultValue1 = nkpd.GroupCode
                };

                var nkpdIndividualGroupTypeList = await this.KeyValueService.GetAllNKPDClassValuesViaKeyTypeIntCodeAsync(nkpdKeyType.KeyTypeIntCode);
                var isClassExisting = nkpdIndividualGroupTypeList.Any(t => t.KeyValueIntCode == newClassType.KeyValueIntCode);

                if (!isClassExisting)
                {
                    resultContext.ResultContextObject = newClassType;
                    await this.KeyValueService.CreateKeyValueAsync(resultContext);
                    updateLogger.Add($"Нова индувидуална група е добавена: Код - [{newClassType.KeyValueIntCode}] Име - [{newClassType.Name}] ");
                }
                else
                {
                    var currentNKPD = nkpdIndividualGroupTypeList.Where(t => t.KeyValueIntCode == newClassType.KeyValueIntCode).FirstOrDefault();
                    string oldName = currentNKPD.Name;
                    var isNKPDNameChange = (currentNKPD.Name != newClassType.Name);
                    if (isNKPDNameChange)
                    {
                        currentNKPD.Name = newClassType.Name;
                        currentNKPD.Description = newClassType.Name;
                        resultContext.ResultContextObject = currentNKPD;
                        await this.KeyValueService.UpdateKeyValueAsync(resultContext);
                        updateLogger.Add($"Името на индивидуалната група[{newClassType.KeyValueIntCode}] е променено: Старо:[{oldName}] - Ново:[{newClassType.Name}]");
                    }
                }
            }
        }

        private async Task InsertNKPDTypesInDb(IEnumerable<NKPDEntry> nkpdList, List<string> updateLogger)
        {

            var NKPDs = await GetAllNKPDAsync();
            var keyTypes = await this.KeyTypeService.GetAllKeyTypesAsync();
            var keyValues = await this.KeyValueService.GetAllAsync();


            var classKeyType = keyTypes.Where(t => t.KeyTypeIntCode == "NKPDClassCode").FirstOrDefault();
            var subClassKeyType = keyTypes.Where(t => t.KeyTypeIntCode == "NKPDSubclassCode").FirstOrDefault();
            var groupKeyType = keyTypes.Where(t => t.KeyTypeIntCode == "NKPDGroupCode").FirstOrDefault();
            var individualGroupKeyType = keyTypes.Where(t => t.KeyTypeIntCode == "NKPDIndividualGroupCode").FirstOrDefault();
            var orderCounter = NKPDs.ToList().Count() + 1;

            foreach (var nkpd in nkpdList)
            {

                var classTypeKeyValue = keyValues.Where(k => k.IdKeyType == classKeyType.IdKeyType && k.KeyValueIntCode == nkpd.ClassCode).FirstOrDefault();
                var subclassTypeKeyValue = keyValues.Where(k => k.IdKeyType == subClassKeyType.IdKeyType && k.KeyValueIntCode == nkpd.SubclassCode).FirstOrDefault();
                var groupypeKeyValue = keyValues.Where(k => k.IdKeyType == groupKeyType.IdKeyType && k.KeyValueIntCode == nkpd.GroupCode).FirstOrDefault();
                var individualGroupTypeKeyValue = keyValues.Where(k => k.IdKeyType == individualGroupKeyType.IdKeyType && k.KeyValueIntCode == nkpd.IndividualGroupCode).FirstOrDefault();

                var newNKPD = new NKPDVM
                {
                    Name = nkpd.Name,
                    Code = nkpd.Code,
                    IdClassCode = classTypeKeyValue.IdKeyValue,
                    IdSubclassCode = subclassTypeKeyValue.IdKeyValue,
                    IdGroupCode = groupypeKeyValue.IdKeyValue,
                    IdIndividualGroupCode = individualGroupTypeKeyValue.IdKeyValue,
                    EducationLevelCode = nkpd.EducationLevelCode
                };

                var isNKPDExistitng = NKPDs.Any(e => e.Code == newNKPD.Code);
                if (!isNKPDExistitng)
                {
                    await CreateNKPDAsync(newNKPD);
                    updateLogger.Add($"Ново НКПД е добавено: Код - [{newNKPD.Code}] Име - [{newNKPD.Name}]");
                }
                else
                {
                    var currentNKPD = NKPDs.FirstOrDefault(e => e.Code == newNKPD.Code);
                    string oldName = currentNKPD.Name;
                    var isNKPDNameChange = (currentNKPD.Name != newNKPD.Name);
                    if (isNKPDNameChange)
                    {
                        currentNKPD.Name = newNKPD.Name;
                        await UpdateNKPD(currentNKPD);
                        updateLogger.Add($"Името на НКПД[{newNKPD.Code}] е променено: Старо:[{oldName}] - Ново:[{newNKPD.Name}]");
                    }
                }

            }

        }
    }
}
