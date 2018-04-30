//-----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//    Copyright (c) 2010-2013 Microsoft Corporation. 
//
//    You must not remove this notice, or any other, from this software.
// </copyright>
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.Research.Yakima.Website.Models.Pager;

namespace Microsoft.Research.Yakima.Website.Models
{
    public class Browse
    {
        public Topic CurrentTopic { get; set; }

        public TopicBrowseNode RootTopic { get; set; }

        public List<DataModel.Topic> TopicAncestry { get; set; }

        public BrowseView CurrentView { get; set; }

        public PagedListNumeric<DataModel.PublicationBase> Publications { get; set; }
    }
}
