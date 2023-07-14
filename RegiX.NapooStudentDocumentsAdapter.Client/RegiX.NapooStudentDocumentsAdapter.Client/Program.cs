using System;
using System.ServiceModel;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.NapooStudentDocumentsAdapter;
using TechnoLogica.RegiX.NapooStudentDocumentsAdapter.AdapterService;

namespace RegiX.NapooStudentDocumentsAdapter.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var client =
                    new ChannelFactory<INapooStudentDocumentsAdapter>(
                        new BasicHttpBinding(),
                        new EndpointAddress("http://localhost:60426/NapooStudentDocumentsAdapter.svc")
                        ).CreateChannel();


                var result =
                    client.GetStudentDocument(
                    new TechnoLogica.RegiX.NapooStudentDocumentsAdapter.StudentDocumentRequestType()
                    {
                        StudentIdentifier = "7409273092",
                        DocumentRegistrationNumber = "10-10"
                    },
                    AccessMatrix.CreateForType(typeof(DocumentsByStudentResponse)),
                    new TechnoLogica.RegiX.Common.DataContracts.AdapterAdditionalParameters()
                    {

                    }
                );

                var result2 =
                    client.GetDocumentsByStudent(
                    new TechnoLogica.RegiX.NapooStudentDocumentsAdapter.DocumentsByStudentRequestType()
                    {
                        StudentIdentifier = "7409273092"
                    },
                    AccessMatrix.CreateForType(typeof(DocumentsByStudentResponse)),
                    new TechnoLogica.RegiX.Common.DataContracts.AdapterAdditionalParameters()
                    {

                    }
                );
            }
            catch( Exception ex ) { 
                Console.Error.WriteLine( ex );
            }
        }
    }
}
