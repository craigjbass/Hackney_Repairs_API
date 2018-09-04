using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackneyRepairs.Actions;
using HackneyRepairs.Interfaces;
using HackneyRepairs.Models;
using Moq;
using Xunit;

namespace HackneyRepairs.Tests.Actions
{
    
    public class NotesActionTests
    {
        Mock<ILoggerAdapter<NotesActions>> _mockLogger;
        public NotesActionTests()
        {
            _mockLogger = new Mock<ILoggerAdapter<NotesActions>>();
        }

        #region get notes feed
        [Fact]
        public async Task get_noted_feed_by_noteID_returns_a_list_of_notes()
        {
            Random rnd = new Random();
            int randomReference = rnd.Next(100000000, 999999999);
            List<DetailedNote> fakeResponse = new List<DetailedNote>
            {
                new DetailedNote()
            };
            Mock<IHackneyWorkOrdersService> _workOrderService = new Mock<IHackneyWorkOrdersService>();
            _workOrderService.Setup(service => service.GetNoteFeed(It.IsAny<int>(), It.IsIn<string>(), It.IsAny<int>()))
                             .Returns(Task.FromResult<IEnumerable<DetailedNote>>(fakeResponse));
            NotesActions notesActions = new NotesActions(_workOrderService.Object,
                                                                        _mockLogger.Object);
            var response = await notesActions.GetNoteFeed(randomReference, "", randomReference);

            Assert.True(response is List<DetailedNote>);
        }
        #endregion
    }
}
