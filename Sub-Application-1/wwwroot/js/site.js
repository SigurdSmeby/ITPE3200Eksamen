function toggleLike(postId) {
    // Example: Simulate an API call to toggle like
    const likeCountElement = document.getElementById(`like-count-${postId}`);
    let currentCount = parseInt(likeCountElement.textContent);

    // Simulate toggling like
    if (likeCountElement.classList.contains('liked')) {
        likeCountElement.textContent = currentCount - 1;
        likeCountElement.classList.remove('liked');
    } else {
        likeCountElement.textContent = currentCount + 1;
        likeCountElement.classList.add('liked');
    }
}

function toggleComments(postId) {
    const commentSection = document.getElementById(`comments-section-${postId}`);
    commentSection.style.display =
        commentSection.style.display === 'none' ? 'block' : 'none';
}

function submitComment(postId) {
    const commentInput = document.getElementById(`comment-input-${postId}`);
    const newComment = commentInput.value.trim();

    if (newComment) {
        // Simulate adding a comment
        const commentSection = document.getElementById(`comments-section-${postId}`);
        const commentDiv = document.createElement('div');
        commentDiv.classList.add('comment');
        commentDiv.innerHTML = `<strong>You</strong>: ${newComment}`;
        commentSection.insertBefore(commentDiv, commentSection.lastElementChild);

        // Clear input
        commentInput.value = '';
    }
}
