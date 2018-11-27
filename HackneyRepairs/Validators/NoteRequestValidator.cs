using System;
using System.Collections.Generic;
using HackneyRepairs.Models;

namespace HackneyRepairs.Validators
{
    public class NoteRequestValidator
    {
        public NoteRequestValidationResult Validate(NoteRequest request)
        {
            var validationResult = new NoteRequestValidationResult(request);

            if (string.IsNullOrWhiteSpace(request.ObjectKey) || !string.Equals(request.ObjectKey.ToLower(), "uhorder"))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a valid object key");
            }
            if (string.IsNullOrWhiteSpace(request.ObjectReference))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide an object reference");
            }
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Please provide a text for the note");
            }
            if (request.Text.Length > 50)
            {
                validationResult.Valid = false;
                validationResult.ErrorMessages.Add("Note text cannot exeed 2000 characters");
            }
            return validationResult;
        }
    }

    public class NoteRequestValidationResult : ValidationResult
    {
        public NoteRequest NoteRequest { get; set; }

        public NoteRequestValidationResult(NoteRequest request)
        {
            ErrorMessages = new List<string>();
            Valid = true;
            NoteRequest = request;
        }
    }
}
