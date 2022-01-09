<template>
  <v-row justify="center" align="center">
    <v-col cols="12" sm="8" md="6">
      <v-card class="logo py-4 d-flex justify-center">
        <NuxtLogo />
        <VuetifyLogo />
      </v-card>
      <v-card>
        <v-card-title class="headline">
          Welcome to the Plant-SCADA
        </v-card-title>
        <v-card-text>
          <p>Plant-SCADA is a visualization tool based on Vue.js.</p>
          <p>
            For more information, check out the <a
              href="https://github.com/GMouaad/dotnetDemo/tree/main/Projects"
              target="_blank"
              rel="noopener noreferrer"
            >
              Github Repository
            </a>.
          </p>
          
          <hr class="my-3">
          <a
            href="https://vuejs.org/"
            target="_blank"
            rel="noopener noreferrer"
          >
            Vue.js
          </a>
          <br>
          <a
            href="https://nuxtjs.org/"
            target="_blank"
            rel="noopener noreferrer"
          >
            Nuxt Documentation
          </a>
          <br>
          <a
            href="https://github.com/nuxt/nuxt.js"
            target="_blank"
            rel="noopener noreferrer"
          >
            Nuxt GitHub
          </a>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="primary"
            nuxt
            to="/about"
          >
            Continue
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-col>
  </v-row>
</template>
