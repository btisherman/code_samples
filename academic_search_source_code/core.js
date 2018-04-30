var Website = {
    hiddenQuery: '',
};

Website.HiddenQuery = function (val) {
    if (val != null) {
        hiddenQuery = val;
    }

    return hiddenQuery;
};

Website.ExpandParentDiv = function (itemId) {
    $('#' + itemId).css('white-space', 'normal');
    $('#' + itemId).css('overflow', 'auto');
    $('#more_' + itemId).hide();
    $('#less_' + itemId).show();
};

Website.CollapseParentDiv = function (itemId) {
    $('#' + itemId).css('white-space', 'nowrap');
    $('#' + itemId).css('overflow', 'hidden');
    $('#more_' + itemId).show();
    $('#less_' + itemId).hide();
};

Website.ResetContentColumnHeight = function () {
    var contentDiv = $("#middleColumn");
    contentDiv.removeAttr("style");
    var headerHeight = $("#header").height();
    var footerHeight = $("#footer").height();

    var contentHeight = $("#contentWrapper").height() - headerHeight - footerHeight;
    if (contentDiv.height() < contentHeight) {
        contentDiv.height(Math.max(contentDiv.parent().height(), contentHeight));
    }
};

/* 
Purpose: Helper function for Ajax GET calls. 
Usage: Website.Async_Get('http://example.com',function(data){$('#mydiv').html(data)});
*/
Website.Async_Get = function (addr, func) {
    $.ajax({
        type: 'GET',
        url: addr,
        success: func
    });
};

/*
Purpose: Decode html string. 
Usage: $('#myText').val(Website.jDecode(str));
*/
Website.jDecode = function(str) {
    return $("<div/>").html(str).text();
};

/*
Purpose: Helper function to add expand/collapse functionality.
Usage: 
1. Decorate container div with class="expandableContainer" and expandable div with class="expandableBlock" 
2. Add data-height & data-width property to container div as shown below:
    ex#1:
        <div class="expandableContainer" data-height="50" data-width="95%">
            <ul class="expandableBlock">
                <li>skks</li>
                <li>skks</li>
            </ul>
        </div>
    ex#2:
        <div class="expandableContainer" data-height="50" data-width="95%">
            <div class="expandableBlock">
                large content
            </div>
        </div>
3. Add following code in parent page which add expand/collapse icons to expandable divs
    $(document).ready(function() {
        Website.AddExpandCollapse();
    });
*/
Website.AddExpandCollapse = function (isWindowResized) {
    var expImg = "<img src='/Content/Images/expand.png' alt='Show More' height='16px' width='16px' style='vertical-align: middle;'/>",
        colImg = "<img src='/Content/Images/collapse.png' alt='Show Less' height='16px' width='16px' style='vertical-align: middle;'/>",
        exItem, defHeight, defWidth, newHeight, imgVerticalAlign = "middle";

    isWindowResized = typeof isWindowResized !== 'undefined' ? isWindowResized : false;

    if (isWindowResized) {
        $('.morelink').parent().remove();
    }
    
    //loop thru all expandableContainer divs and insert div with expand image
    $('.expandableContainer').each(function () {
        if ($(this).find(".morelink").length === 0) { //check if anchor tag with class="morelink" doesn't exist
            exItem = $(this).find(".expandableBlock");
            defHeight = $(this).attr("data-height");
            defWidth = $(this).attr("data-width");

            imgVerticalAlign = $(this).attr("data-valign");
            if (typeof imgVerticalAlign === "undefined") {
                expImg = "<img src='/Content/Images/expand.png' alt='Show More' height='16px' width='16px' style='vertical-align: middle;'/>";
            } else {
                expImg = expImg.replace("middle", imgVerticalAlign);
            }

            //measure original height of expandable item
            newHeight = exItem.height('auto').height();
            //put back old height
            exItem.height(defHeight);

            if (defHeight < newHeight) {
                var html = $(this).html() +
                    '<div style="display:inline; position: absolute; right: 15px; bottom: 0; line-height:' +
                    defHeight + 'px;"><a href="javascript:void(0)" class="morelink">' + expImg + '</a></div>';
                $(this).html(html);
            }

            $(this).css("width", defWidth);
        }
    });

    //unbind and attach click event handler for expand/collapse
    $(".morelink").unbind("click").click(function () {
        imgVerticalAlign = $(this).parents(".expandableContainer").attr("data-valign");
        if (imgVerticalAlign !== undefined) {
            expImg = expImg.replace("middle", imgVerticalAlign);
            colImg = colImg.replace("middle", imgVerticalAlign);
        }
        if ($(this).hasClass("less")) {
            $(this).removeClass("less");
            $(this).html(expImg);
            var origHeight = $(this).parents(".expandableContainer").attr("data-height");
            $(this).parents(".expandableContainer").find(".expandableBlock").css('height', origHeight);
        } else {
            $(this).addClass("less");
            $(this).html(colImg);
            $(this).parents(".expandableContainer").find(".expandableBlock").css('height', 'auto');
        }
        
        Website.ResetContentColumnHeight();
        return false;
    });        
};

Website.AddSharedExpander = function (isWindowResized, expandText, collapseText) {
    var expImg = "<img src='/Content/Images/expand.png' alt='" + expandText + "' title='" + expandText + "' />",
        colImg = "<img src='/Content/Images/collapse.png' alt='" + collapseText + "' title='" + collapseText + "' />";

    isWindowResized = typeof isWindowResized !== 'undefined' ? isWindowResized : false;

    if (isWindowResized) {
        $('.morelinkContainer').remove();
    }
    
    //loop thru all expandableBlock divs and insert div with expand image
    $('.expandableContainer').each(function () {
        var $parent = $(this);
        $(this).find('.expandableBlock').each(function () {
            var $this = $(this);

            if (this.nodeName === "DIV") {
                var element = $this
                                .clone()
                                .css({ display: 'inline', width: 'auto', visibility: 'hidden' })
                                .appendTo('body');

                //check if ellipsis applied && anchor tag with class="morelink" doesn't exist
                if (element.width() > $this.width()) {
                    if ($parent.find(".morelink").length === 0) {
                        var html = $parent.html() +
                            '<div class="morelinkContainer"><a href="javascript:void(0)" class="morelink">' + expImg + '</a></div>';
                        $parent.html(html);
                    }
                }
                element.remove();
            }
        });
    });


    //unbind and attach click event handler for expand/collapse
    $(".morelink").unbind("click").click(function () {
        if ($(this).hasClass("less")) {
            $(this).removeClass("less");
            $(this).html(expImg);

            $(this).parents(".expandableContainer").find(".expandableBlock").css('white-space', 'nowrap').css('overflow', 'hidden');
        } else {
            $(this).addClass("less");
            $(this).html(colImg);
            $(this).parents(".expandableContainer").find(".expandableBlock").css('white-space', 'normal').css('overflow', 'auto');
        }

        Website.ResetContentColumnHeight();
        return false;
    });
};

Website.getMoreSearchResults = function () {
    $.ajax(
        {
            type: "GET",
            url: $('#LoadMoreUrl').val() + location.search + "&s=" + $('#skip').val(),
            beforeSend: function () {
                $('#loadingImage').show();
            },
            success: function (data) {
                $('#resultsDiv').append(data);
                Website.AddSharedExpander();
            },
            complete: function () {
                $('#loadingImage').hide();
                if ($('#searchResponseCount').val() <= 0) {
                    $('#loadMoreLink').hide();
                }
            }
        });
};

Website.SortByClicked = function (sortType) {
    $("#hiddenSelectedSort").val(sortType);
};

Website.SearchCheckboxClicked = function (selectedCheckbox, resultControlId) {
    var selectedChkValue = selectedCheckbox.value;
    if (selectedCheckbox.checked) {
        $("#" + resultControlId).val($("#" + resultControlId).val() | selectedChkValue);
    } else {
        $("#" + resultControlId).val($("#" + resultControlId).val() ^ selectedChkValue);
    }
};

Website.GetUpdatedSearchUrl = function (form) {
    
    // start by making sure #q matches the HiddenQuery to handle cases when the Back button has been used
    $('#q').val(Website.jDecode(Website.HiddenQuery()));

    var params = [];
    if ($("#textStartYear").length && $("#textStartYear").val().length) {
        params.push('<input id="sYr" name="sYr" type="hidden" value="' + $("#textStartYear").val() + '" />');
    }

    if ($("#textEndYear").length && $("#textEndYear").val().length) {
        params.push('<input id="eYr" name="eYr" type="hidden" value="' + $("#textEndYear").val() + '" />');
    }
    
    if ($("#hiddenSelectedSort").length) {
        params.push('<input id="order" name="order" type="hidden" value="' + $("#hiddenSelectedSort").val() + '" />');
    }

    if ($("#hiddenSearchFields").length) {
        params.push('<input id="fld" name="fld" type="hidden" value="' + $("#hiddenSearchFields").val() + '" />');
    }

    if ($("#hiddenSearchAccess").length) {
        params.push('<input id="acs" name="acs" type="hidden" value="' + $("#hiddenSearchAccess").val() + '" />');
    }

    if ($("#cbIncludeNoYearPubs").length) {
        params.push('<input id="noYr" name="noYr" type="hidden" value="' + $("#cbIncludeNoYearPubs").is(':checked') + '" />');
    }

    $(form).append(params.join());
};

Website.RequerySearchResults = function () {
    if (Website.HiddenQuery() === "") {
    } else {
        $('#searchForm').submit();
    }
};

Website.HideLoadMoreLinkIfApplicable = function () {
    if ($('input[data-nextPageUrl]').last().val().length == 0) {
        $('#loadMoreLink').hide();
    }
};

Website.EnableSelectablePubItem = function () {
    $('.vcard').unbind("click").click(function (e) {
        if (e.target.tagName != 'INPUT') {
            $('span.custom.checkbox', this).toggleClass('checked');

            var $cb = $(':checkbox', this);
            if ($cb.is(':checked')) {
                $cb.prop('checked', false);
            } else {
                $cb.prop('checked', true);
            }
        }
        return false;
    });
};


var Author = {};

Author.AddRemovePapers = function (authorId, actionType) {
    var checkValues = $('input[name=selPaperCheckBox]:checked').map(function () {
        return $(this).val();
    }).get();
    
    if (checkValues.length == 0) {
        $('#authPapersAlertBox' + actionType).show();
        if ($('#authPapersAlertBox' + actionType).hasClass("success")) {
            $('#authPapersAlertBox' + actionType).removeClass("success");
        }
        $('#authPapersAlertBox' + actionType).addClass('alert');
        $('#authPapersAlertMsg' + actionType).html(window.authPapersAlertMsgs[0]);
        return;
    } else {
        $('#authPapersAlertBox' + actionType).hide();
    }
    
    $.ajax({
        type: "POST",
        url: "/author/" + authorId + "/UpdatePapers",
        data: "action=" + actionType + "&paperIds=" + checkValues,
        success: function (msg) {
            $('#authPapersAlertBox' + actionType).show();
            if (msg) {
                if ($('#authPapersAlertBox' + actionType).hasClass("alert")) {
                    $('#authPapersAlertBox' + actionType).removeClass("alert");
                }
                $('#authPapersAlertBox' + actionType).addClass('success');
                $('#authPapersAlertMsg' + actionType).html(window.authPapersAlertMsgs[1]);
            } else {
                $('#authPapersAlertBox' + actionType).addClass('alert');
                $('#authPapersAlertMsg' + actionType).html(window.authPapersAlertMsgs[2]);
            }
        }
    });
};

Author.EnableOrganizationsAutoComplete = function (keywordSearchUrl) {
    $("#organization").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: keywordSearchUrl,
                type: "POST",
                dataType: "json",
                data: { term: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Name, value: item.Name, id: item.Id };
                    }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            $('#organizationId').val(ui.item.id);
            $("#organization").val(ui.item.label);
            return false;
        },
        appendTo: "#orgResultsContainer",
        open: Author.ResizeOrganizationsResultsMenu
    });
};

Author.ResizeOrganizationsResultsMenu = function () {
    $('#orgResultsContainer .ui-menu').width($('#organization').width() + 8);
};