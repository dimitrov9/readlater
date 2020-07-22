var apiUrl;
var _categories;

$(document).ready(function () {
    apiUrl = $("#apiUrl").data("value");
    fetchBookmarks();

    $("#create-bookmark-form").on("submit", function (e) {
        saveBookmark();
        e.preventDefault();
    });
});

function fetchBookmarks() {
    $.ajax({
        type: "GET",
        url: apiUrl + "/bookmarks",
        cache: false,
        crossDomain: false,
        xhrFields: {
            withCredentials: true
        },
        success: function (data) {
            $("#bookmarks-loader").remove();
            const rows = data.map(bookmark => mapBookmarkToTableRow(bookmark));
            $("#table-header").after(rows);
        }
    });
}

async function getCategories() {
    if (_categories) {
        return Promise.resolve(_categories);
    } else {
        _categories = await $.ajax({
            type: "GET",
            url: `${apiUrl}/categories`,
            cache: false,
            crossDomain: false,
            xhrFields: {
                withCredentials: true
            }
        });

        return _categories;
    }
}

async function showCreateForm() {
    $("#btn-create").hide();
    let html = `
    <tr id="create-row">
        <td>
            <div class="form-group">
                <input type="url" id="input-url" class="form-control" required maxlength=500 placeholder="URL" form="create-bookmark-form" />
            </div>
            <div class="form-group">
                <input type="text" id="input-description" class="form-control" placeholder="Description" form="create-bookmark-form" />
            </div>
        </td>
        <td>
            <div class="form-group">
                <select id="input-category" class="form-control" form="create-bookmark-form">
                    <option>Loading...</option>
                </select>
            </div>
            <div class="form-group">
                <input type="hidden" id="input-new-category" class="form-control" placeholder="Create New Category" form="create-bookmark-form" />
            </div>
        </td>
        <td class="action-buttons">
            <button class="btn btn-success" form="create-bookmark-form" type="submit" >Save</button>
            <button class="btn btn-danger" onclick="hideCreateForm()" type="button" >Cancel</button>
        </td>
    </tr>`

    $("#table-header").after(html);
    const categories = await getCategories();
    $("#input-category").html(mapCategoriesToSelectOptions(categories));

    $("#input-category").change(function () {
        var selected = $(this).children("option:selected").val();

        if (selected === "new") {
            $("#input-new-category").attr("type", "text");
        } else {
            $("#input-new-category").attr("type", "hidden");
        }
    });
}

function mapCategoriesToSelectOptions(categories, selected) {
    return [
        `<option value ${selected ? "" : "selected"} > --No Category-- </option > `,
        `<option value="new" > New Category </option>`,
        ...categories.map(c => `<option value="${c.ID}" ${selected === c.Name ? "selected" : ""}>${c.Name}</option>`)
    ];
}

function hideCreateForm() {
    $("#create-row").remove();
    $("#btn-create").show();
}

function saveBookmark() {
    $("#create-bookmark-form").validate();
    if (!$("#create-bookmark-form").valid()) return;

    const data = {
        url: $("#input-url").val(),
        description: $("#input-description").val(),
    }

    const category = $("#input-category").children("option:selected").val();

    if (category) {
        if (category === "new") {
            data.newCategory = $("#input-new-category").val();
            _categories = null;
        } else {
            data.categoryId = +category;
        }
    }

    $.ajax({
        type: "POST",
        url: apiUrl + "/bookmarks",
        cache: false,
        crossDomain: false,
        xhrFields: {
            withCredentials: true
        },
        data: data,
        success: function (data) {
            hideCreateForm();
            const row = mapBookmarkToTableRow(data);
            $("#table-header").after(row);
        }
    });
}

function mapBookmarkToTableRow(bookmark) {
    return `    
    <tr id="bookmark-row-${bookmark.ID}">
        <td class="col-md-6">
            <a href="${bookmark.URL}" >${bookmark.ShortDescription}</a>
        </td>
        <td class="col-md-3">${bookmark.Category?.Name ?? ''}</td>
        <td class="col-md-3 action-buttons">
            <button class="btn btn-default" onclick="editBookmark(${bookmark.ID})">Edit</button>
            <button class="btn btn-danger" onclick="deleteBookmark(${bookmark.ID})">Delete</button>
        </td>
    </tr>`;
}

function deleteBookmark(bookmarkId) {
    $.ajax({
        url: `${apiUrl}/bookmarks/${bookmarkId}`,
        type: 'DELETE',
        cache: false,
        crossDomain: false,
        xhrFields: {
            withCredentials: true
        },
        success: function () {
            $(`#bookmark-row-${bookmarkId}`).remove();
        }
    });
}

async function editBookmark(bookmarkId) {
    $("#create-bookmark-form").after(`<form id="edit-bookmark-form-${bookmarkId}"></form >`);

    const bookmarkRow = $(`#bookmark-row-${bookmarkId}`);
    const anchor = bookmarkRow.find("a");
    const desc = anchor.html();
    const url = anchor.attr("href");
    const categoryName = bookmarkRow.children().eq(1).html();

    const formHtml = `
    <tr id="edit-row-${bookmarkId}">
        <td>
            <div class="form-group">
                <input type="url" id="input-url-${bookmarkId}" class="form-control" required maxlength=500 
                    placeholder="URL" form="edit-bookmark-form-${bookmarkId}" value="${url}" />
            </div>
            <div class="form-group">
                <input type="text" id="input-description-${bookmarkId}" class="form-control" 
                    placeholder="Description" form="edit-bookmark-form-${bookmarkId}" value="${desc}"/>
            </div>
        </td>
        <td>
            <div class="form-group">
                <select id="input-category-${bookmarkId}" class="form-control" form="edit-bookmark-form-${bookmarkId}">
                    <option>Loading...</option>
                </select>
            </div>
            <div class="form-group">
                <input type="hidden" id="input-new-category-${bookmarkId}" class="form-control" placeholder="Create New Category" form="create-bookmark-form-${bookmarkId}" />
            </div>
        </td>
        <td class="action-buttons">
            <button class="btn btn-success" form="edit-bookmark-form-${bookmarkId}" type="submit">Save</button>
            <button class="btn btn-danger" onclick="hideEditForm(${bookmarkId})" type="button">Cancel</button>
        </td>
    </tr>`;

    bookmarkRow.hide();
    bookmarkRow.after(formHtml);
    const categories = await getCategories();
    $(`#input-category-${bookmarkId}`).html(mapCategoriesToSelectOptions(categories, categoryName));

    $(`#edit-bookmark-form-${bookmarkId}`).on("submit", function (e) {
        submitEditBookmark(bookmarkId);
        e.preventDefault();
    });

    $(`#input-category-${bookmarkId}`).change(function () {
        var selected = $(this).children("option:selected").val();

        if (selected === "new") {
            $(`#input-new-category-${bookmarkId}`).attr("type", "text");
        } else {
            $(`#input-new-category-${bookmarkId}`).attr("type", "hidden");
        }
    });
}

function hideEditForm(bookmarkId) {
    $(`#edit-row-${bookmarkId}`).remove();
    $(`#edit-bookmark-form-${bookmarkId}`).remove();
    $(`#bookmark-row-${bookmarkId}`).show();
}

function submitEditBookmark(bookmarkId) {
    $(`#edit-bookmark-form-${bookmarkId}`).validate();
    if (!$(`#edit-bookmark-form-${bookmarkId}`).valid()) return;

    const data = {
        url: $(`#input-url-${bookmarkId}`).val(),
        description: $(`#input-description-${bookmarkId}`).val(),
    }

    const category = $(`#input-category-${bookmarkId}`).children("option:selected").val();

    if (category) {
        if (category === "new") {
            data.newCategory = $(`#input-new-category-${bookmarkId}`).val();
            _categories = null;
        } else {
            data.categoryId = +category;
        }
    }

    $.ajax({
        url: `${apiUrl}/bookmarks/${bookmarkId}`,
        type: 'PUT',
        cache: false,
        crossDomain: false,
        xhrFields: {
            withCredentials: true
        },
        data: data,
        success: function (bookmark) {
            hideEditForm(bookmarkId);
            $(`#bookmark-row-${bookmarkId}`).after(mapBookmarkToTableRow(bookmark));
            $(`#bookmark-row-${bookmarkId}`).remove();
        }
    });
}