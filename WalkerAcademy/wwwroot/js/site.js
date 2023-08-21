var itemsPerPage = 20;
var currentPage = 1;

// Sort list alphabetically
allItems.sort((a, b) => {
    if (a.name < b.name) {
        return -1;
    }
    if (a.name > b.name) {
        return 1;
    }
    return 0;  // names are equal
});

function displayPage(page, searchTerm = '') {
    var startIndex = (page - 1) * itemsPerPage;
    var endIndex = startIndex + itemsPerPage;

    // Clear current list
    $('#itemList').empty();

    let filteredItems = allItems.filter(item => {
        return item.name.toLowerCase().includes(searchTerm.toLowerCase());
    });

    // Display items for the current page
    for (var i = startIndex; i < endIndex && i < filteredItems.length; i++) {
        $('#itemList').append(
            `
                   <li>
                          <a href="#" class="text-decoration-none item-link" data-bs-toggle="modal" data-bs-target="#modal-${filteredItems[i].id}">${filteredItems[i].name}</a>

                      <!-- Bootstrap Modal -->
                          <div class="modal fade" id="modal-${filteredItems[i].id}" tabindex="-1">
                          <div class="modal-dialog">
                              <div class="modal-content">
                                  <div class="modal-header">
                                          <h5 class="modal-title">${filteredItems[i].name}</h5>
                                      <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                  </div>
                                  <div class="modal-body">
                                     <p>${filteredItems[i].description}</p>
                                     <br />
                                     <div class="item-image d-flex justify-content-center">
                                        <img src="${filteredItems[i].image}" style="width: 90%; height: 45%" alt="${filteredItems[i].name} image">
                                     </div>
                                  </div>
                                  <div class="modal-footer">
                                      <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                                  </div>
                              </div>
                          </div>
                      </div>
                      <!-- End of Bootstrap Modal -->
                   </li>
                `);
    }

    // Update pagination controls (Disable "Next" button if on the last page)
    updatePaginationControls(filteredItems.length);
}

function updatePaginationControls(filteredLength) {
    if (currentPage * itemsPerPage >= filteredLength) {
        $('#nextButton').prop('disabled', true);
    } else {
        $('#nextButton').prop('disabled', false);
    }

    if (currentPage === 1) {
        $('#prevButton').prop('disabled', true);
    } else {
        $('#prevButton').prop('disabled', false);
    }
}


// Bind pagination controls to the displayPage function
$('#nextButton').click(function () {
    currentPage++;
    displayPage(currentPage);
});

$('#prevButton').click(function () {
    currentPage--;
    displayPage(currentPage);
});

// Initially display the first page
$(document).ready(function () {
    displayPage(1);
});

// Event listener for search box
$('#search').on('input', function () {
    var searchTerm = $(this).val();
    currentPage = 1;
    displayPage(currentPage, searchTerm);
});