// get AntiForgeryToken using jquery
const token = $('input[name="__RequestVerificationToken"]').val();

const uploadElement = $('#upload-modal')[0];
if (uploadElement) {
    uploadElement.addEventListener('show.bs.modal', function (event) {
        // Button that triggered the modal
        var button = event.relatedTarget
        // Extract info from data-bs-* attributes
        var recipient = button.getAttribute('data-bs-whatever')
        // If necessary, you could initiate an AJAX request here
        // and then do the updating in a callback.
        //
        // Update the modal's content.
        var modalTitle = uploadElement.querySelector('.modal-title')
        var modalBodyInput = uploadElement.querySelector('.modal-body input')

        modalTitle.textContent = 'New message to ' + recipient
        modalBodyInput.value = recipient
    })
}

// Initialize DropZone for file uploads within the modal if the element exists.
if (document.getElementById("upload-dropzone")) {
    const myDropzone = new Dropzone("#upload-dropzone", {
        url: "/api/upload", // backend endpoint to handle uploads
        paramName: "file",
        maxFilesize: 5, // max filesize in MB
        acceptedFiles: "image/*,application/pdf", // adjust accepted file types as needed
        init: function() {
            this.on("success", function(file, response) {
                console.log("File uploaded successfully", response);
            });
            this.on("error", function(file, errorMessage) {
                console.error("File upload error", errorMessage);
            });
        }
    });
}

// Debounce helper function
function debounce(func, delay) {
    let timeout;
    return function(...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), delay);
    };
}

// Replace gridjs initialization with DataTables initialization following RofoDto structure
if (document.getElementById("datatable-rofo") || document.getElementById("datatables-rofo")) {
    $('#datatables-rofo').DataTable({
        searchDelay: 1000,
        ajax: {
            url: `/api/RofoApi/GetRofoPagedList`,
            data: function(d) {
                // Create a request object following PagedListRequestDto<RofoDto>
                // d.pageSize = d.length;
                // d.pageNumber = (d.start / d.length) + 1;
                //d.search = d.search.value;
                //return d;
                const query = {
                    pageNumber: (d.start / d.length) + 1,
                    pageSize: d.length,
                };
                if (d.search && d.search.value) {
                    query.Search = d.search.value;
                }
                if (d.order && d.order.length) {
                    query.SortBy = d.columns[d.order[0].column].data;
                    query.IsSortAscending = d.order[0].dir === 'asc';
                }
                
                d.columns.forEach((column, _) => {
                    if (column.search && column.search.value) {
                        // Encode the search value for URL safety.
                        query[`Filter.${column.data}`] = column.search.value.replace(/\*/g, '%2A');
                    }
                });
                return query;
            },
            beforeSend: function(xhr) {
                xhr.setRequestHeader("RequestVerificationToken", token);
            },
            dataSrc: function(json) {
                // Check if response follows ServiceResponse<PagedListDto<RofoDto>> structure:
                if (json.data && Array.isArray(json.data.items)) {
                    json.recordsTotal = json.data.totalCount;
                    json.recordsFiltered = json.data.totalCount;
                    return json.data.items;
                }
                // Fallback: if json itself is an array.
                return json;
            }
        },
        columns: [
            { data: 'room', width: '80px' },
            { data: 'rofoDate', width: '120px' },
            { data: 'itemId', width: '100px' },
            { data: 'itemName', width: '150px' },
            { data: 'qty', width: '80px' },
            { data: 'qtyRem', width: '80px' },
            { data: 'createdBy', width: '120px' },
            { data: 'createdDate', width: '120px' },
            { data: 'recId', width: '80px' },
        ],
        processing: true,
        serverSide: true,
        paging: true,
        searching: true,
        ordering: true,
        initComplete: function () {
            this.api()
                .columns()
                .every(function () {
                    let column = this;
                    let title = column.footer().textContent;

                    // Create input element
                    let input = document.createElement('input');
                    input.placeholder = title;
                    column.footer().replaceChildren(input);

                    // Debounced event listener for user input (1 second delay)
                    input.addEventListener('keyup', debounce(() => {
                        if (column.search() !== input.value) {
                            column.search(input.value).draw();
                        }
                    }, 1000));
                });
        }
    });
}