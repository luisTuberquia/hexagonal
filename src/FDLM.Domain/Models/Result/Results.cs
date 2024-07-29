using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FDLM.Domain.Models.Result
{
    public class Results<T>
    {
        private List<FdlmError> _errors;
        public T Result { get; set; }
        public long TotalItemsReturned { get; set; }
        public long TotalItemsInDataBase { get; set; }

        public List<FdlmError> Errors { get {
                if (_errors == null) 
                {
                    _errors = new List<FdlmError>(); 
                }
                return _errors;
            }
            set
            {
                _errors = value;
            }
        }

        public Results()
        {
            _errors = new List<FdlmError>();
        }

        public Results(T result)
        {
            Result = result;
            _errors = new List<FdlmError>();
        }

        public bool IsSuccess
        {
            get
            {
                return !Errors.Any();
            }
        }

        public bool HasErrors
        {
            get { 
                return !IsSuccess; 
            }
        }

        
        public string getErrorsAsString()
        {
            return $"Total Errors:{Errors.Count}, Errors:{string.Join(", ", Errors.Select(e => e.Message))}";
        }

        public Results<T> AddErrors(IEnumerable<FdlmError> errors)
        {
            if (errors != null)
            {
                Errors.AddRange(errors);
            }
            
            return this;
        }

        public Results<T> AddError(FdlmError error)
        {
            Errors.Add(error);
            return this;
        }

        public Results<T> AddError(string message)
        {
            var error = new FdlmError(ErrorCode.GENERIC_ERROR, message);
            AddError(error);
            return this;
        }

        public Results<T> AddError(ErrorCode errorCode, string message)
        {
            var error = new FdlmError(errorCode, message);
            AddError(error);
            return this;
        }

        public Results<T> AddError(Exception exception)
        {
            var error = new FdlmError(ErrorCode.GENERIC_ERROR, exception.Message, exception);
            AddError(error);
            return this;
        }

        public Results<T> AddError(string message, Exception exception)
        {
            var error = new FdlmError(ErrorCode.GENERIC_ERROR, message, exception);
            AddError(error);
            return this;
        }
    }
}
