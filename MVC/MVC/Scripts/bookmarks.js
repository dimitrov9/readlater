var apiUrl;
var _categories;
var categories$;

$(document).ready(function () {
    apiUrl = $("#apiUrl").data("value");
    fetchBookmarks();
    categories$ = new Promise((resolve, reject) => {
        if (_categories) {
            resolve(_categories);
        } else {
            $.get(`${apiUrl}/categories`).then(resolve, reject);
        }
    });

    $("#create-bookmark-form").on("submit", function (e) {
        saveBookmark();
        e.preventDefault();
    });
});

function fetchBookmarks() {
    $.get(apiUrl + "/bookmarks", function (data) {
        $("#bookmarks-loader").remove();
        const rows = data.map(bookmark => mapBookmarkToTableRow(bookmark));
        $("#table-header").after(rows);
    });
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
        <td class="form-group">
            <select id="input-category" class="form-control" form="create-bookmark-form">
                <option>Loading...</option>
            </select>
        </td>
        <td class="action-buttons">
            <button class="btn btn-success" form="create-bookmark-form" type="submit" >Save</button>
            <button class="btn btn-danger" onclick="hideCreateForm()" type="button" >Cancel</button>
        </td>
    </tr>`

    $("#table-header").after(html);
    categories$.then(categories => $("#input-category").html(mapCategoriesToSelectOptions(categories)));
}

function mapCategoriesToSelectOptions(categories, selected) {
    return [
        `<option value ${selected ? "" : "selected"} > --No Category-- </option > `,
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

    const url = $("#input-url").val();
    const desc = $("#input-description").val();
    const categoryId = +$("#input-category").children("option:selected").val();

    $.post(apiUrl + "/bookmarks", {
        url: url,
        description: desc,
        categoryId: categoryId
    }, function (data) {
        hideCreateForm();
        const row = mapBookmarkToTableRow(data);
        $("#table-header").after(row);
    });
}

mapBookmarkToTableRow = (bookmark) => `    
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

function deleteBookmark(bookmarkId) {
    $.ajax({
        url: `${apiUrl}/bookmarks/${bookmarkId}`,
        type: 'DELETE',
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
        <td class="form-group">
            <select id="input-category-${bookmarkId}" class="form-control" form="edit-bookmark-form-${bookmarkId}">
                <option>Loading...</option>
            </select>
        </td>
        <td class="action-buttons">
            <button class="btn btn-success" form="edit-bookmark-form-${bookmarkId}" type="submit">Save</button>
            <button class="btn btn-danger" onclick="hideEditForm(${bookmarkId})" type="button">Cancel</button>
        </td>
    </tr>`;

    bookmarkRow.hide();
    bookmarkRow.after(formHtml);
    categories$.then(categories => $(`#input-category-${bookmarkId}`).html(mapCategoriesToSelectOptions(categories, categoryName)));


    $(`#edit-bookmark-form-${bookmarkId}`).on("submit", function (e) {
        submitEditBookmark(bookmarkId);
        e.preventDefault();
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

    const url = $(`#input-url-${bookmarkId}`).val();
    const desc = $(`#input-description-${bookmarkId}`).val();
    const categoryId = +$(`#input-category-${bookmarkId}`).children("option:selected").val();

    $.ajax({
        url: `${apiUrl}/bookmarks/${bookmarkId}`,
        type: 'PUT',
        data: {
            url: url,
            description: desc,
            categoryId: categoryId
        },
        success: function (bookmark) {
            hideEditForm(bookmarkId);
            $(`#bookmark-row-${bookmarkId}`).after(mapBookmarkToTableRow(bookmark));
            $(`#bookmark-row-${bookmarkId}`).remove();
        }
    });
}