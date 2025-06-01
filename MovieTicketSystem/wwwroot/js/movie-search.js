// Movie search functionality
document.addEventListener('DOMContentLoaded', function () {
  // Elements
  const searchForm = document.getElementById('movieSearchForm');
  const searchInput = document.getElementById('searchInput');
  const filterRating = document.getElementById('filterRating');
  const sortOrder = document.getElementById('sortOrder');
  const movieContainer = document.getElementById('movieContainer');
  const searchStats = document.getElementById('searchStats');
  const loadingIndicator = document.getElementById('loadingIndicator');

  // Variables to control search timing
  let searchTimeout;
  const searchDelay = 500; // milliseconds

  // Function to update search stats
  function updateSearchStats(count, searchTerm, rating) {
    if (searchStats) {
      let statsText = `Showing ${count} movies`;

      if (searchTerm && rating) {
        statsText += ` matching "${searchTerm}" with rating "${rating}"`;
      } else if (searchTerm) {
        statsText += ` matching "${searchTerm}"`;
      } else if (rating) {
        statsText += ` with rating "${rating}"`;
      }

      searchStats.textContent = statsText;
    }
  }

  // Function to show loading indicator
  function showLoading() {
    if (loadingIndicator) {
      loadingIndicator.classList.remove('hidden');
    }
  }

  // Function to hide loading indicator
  function hideLoading() {
    if (loadingIndicator) {
      loadingIndicator.classList.add('hidden');
    }
  }

  // Function to perform search
  function performSearch() {
    if (!searchForm) return;

    const searchTerm = searchInput ? searchInput.value : '';
    const rating = filterRating ? filterRating.value : '';
    const sort = sortOrder ? sortOrder.value : 'newest';

    // Update the URL with search parameters
    const url = new URL(window.location);

    if (searchTerm) {
      url.searchParams.set('SearchTerm', searchTerm);
    } else {
      url.searchParams.delete('SearchTerm');
    }

    if (rating) {
      url.searchParams.set('Rating', rating);
    } else {
      url.searchParams.delete('Rating');
    }

    if (sort !== 'newest') {
      url.searchParams.set('SortBy', sort);
    } else {
      url.searchParams.delete('SortBy');
    }

    // Use history.pushState to update the URL without refreshing the page
    history.pushState({}, '', url);

    // Show loading indicator
    showLoading();

    // Fetch results from server with AJAX
    fetch(url)
      .then((response) => response.text())
      .then((html) => {
        // Create a temporary element to parse the HTML
        const parser = new DOMParser();
        const doc = parser.parseFromString(html, 'text/html');

        // Extract the movie container content
        const newMovieContainer = doc.getElementById('movieContainer');
        if (newMovieContainer && movieContainer) {
          movieContainer.innerHTML = newMovieContainer.innerHTML;

          // Update movie count stats
          const movieCount =
            movieContainer.querySelectorAll('.movie-card').length;
          updateSearchStats(movieCount, searchTerm, rating);

          // Reset image error handlers
          document
            .querySelectorAll('img[src^="/images/movies/"]')
            .forEach(function (img) {
              img.onerror = function () {
                handleImageError(this);
              };
            });
        }

        // Hide loading indicator
        hideLoading();
      })
      .catch((error) => {
        console.error('Error fetching search results:', error);
        hideLoading();
      });
  }

  // Set up event listeners for live search
  if (searchInput) {
    searchInput.addEventListener('input', function () {
      clearTimeout(searchTimeout);
      searchTimeout = setTimeout(performSearch, searchDelay);
    });
  }

  if (filterRating) {
    filterRating.addEventListener('change', performSearch);
  }

  if (sortOrder) {
    sortOrder.addEventListener('change', performSearch);
  }

  // Prevent default form submission since we're handling it with AJAX
  if (searchForm) {
    searchForm.addEventListener('submit', function (e) {
      e.preventDefault();
      clearTimeout(searchTimeout);
      performSearch();
    });
  }
});
