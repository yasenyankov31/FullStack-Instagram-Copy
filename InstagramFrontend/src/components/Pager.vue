<template>
    <div class="pagination">
        <a @click="changePage(currentPage > 1 ? currentPage -= 1 : 1)">&laquo;</a>
        <a v-for="pageNumber in maxPages" :key="pageNumber" :class="{ active: pageNumber === currentPage }"
            @click="changePage(pageNumber)">{{ pageNumber }}</a>
        <a @click="changePage(currentPage < maxPages ? currentPage += 1 : maxPages)">&raquo;</a>
    </div>
</template>
<script>
export default {
    props: ['maxPages', 'loadData', 'currentPage'],
    data() {
        return {
            currentPage: this.currentPage
        }
    },
    methods: {
        changePage(pageNumber) {
            this.currentPage = pageNumber
            this.loadData()
        }
    }

}
</script>
<style >
.pagination {
    display: inline-block;
}

.pagination a {
    color: black;
    float: left;
    padding: 8px 16px;
    text-decoration: none;
}

.pagination a.active {
    background: linear-gradient(90deg, rgba(139, 0, 255, 1) 0%, rgba(221, 149, 20, 1) 83%);
    border-radius: 20%;
    color: white;
}

.pagination a:hover:not(.active) {
    background-color: #ddd;
    border-radius: 20%;
}
</style>