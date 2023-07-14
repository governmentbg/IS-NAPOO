using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISNAPOO.Core.ViewModels.Rating;

namespace ISNAPOO.Core.ValidationAttributes
{
    public class CustomIndicatorValidationAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IndicatorVM model = validationContext.ObjectInstance as IndicatorVM;
            #region Numerical
            if (model != null && model.IndicatorDetails.DefaultValue1 == "Numerical")
            {
                if (validationContext.MemberName == "RangeFrom")
                {
                    if (model.RangeFrom == null)
                    {
                        return new ValidationResult("Полето \"Диапазон от\" е задължително");
                    }
                    else
                    {
                        if (model.RangeFrom < 0)
                        {
                            return new ValidationResult(ErrorMessage);

                        }
                    }
                }

                else if (validationContext.MemberName == "RangeTo")
                {
                    if (model.RangeTo == null)
                    {
                        return new ValidationResult("Полето \"Диапазон до\" е задължително");
                    }
                    else
                    {
                        if (model.RangeTo < 0)
                        {
                            return new ValidationResult(ErrorMessage);
                        }
                    }
                }

                else if (validationContext.MemberName == "Points")
                {
                    if (model.Points == null)
                    {
                        return new ValidationResult("Полето \"Точки\" е задължително");
                    }
                    else
                    {
                        if (model.Points < 0)
                        {
                            return new ValidationResult(ErrorMessage);
                        }
                    }
                }

                if (validationContext.MemberName == "Points")
                {
                    if (model.RangeFrom >= model.RangeTo)
                    {
                        return new ValidationResult("Невалиден диапазон");
                    }
                }

                return ValidationResult.Success;
            }
            #endregion

            #region Quality
            else if (model != null && model.IndicatorDetails.DefaultValue1 == "Quality")
            {
                if (validationContext.MemberName == "PointsYes")
                {
                    if (model.PointsYes == null)
                    {
                        return new ValidationResult("Полето \"Точки Да\" е задължително");
                    }
                    else
                    {
                        if (model.PointsYes < 0)
                        {
                            return new ValidationResult(ErrorMessage);

                        }
                    }
                }
                else if (validationContext.MemberName == "PointsNo")
                {
                    if (model.PointsNo == null)
                    {
                        return new ValidationResult("Полето \"Точки Не\" е задължително");
                    }
                    else
                    {
                        if (model.PointsNo < 0)
                        {
                            return new ValidationResult(ErrorMessage);

                        }
                    }
                }
                #endregion

                return ValidationResult.Success;
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
