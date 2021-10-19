<template>
  <v-container>
    <v-row>
      <v-col>
        <v-banner align="center">Floor :</v-banner>
      </v-col>
      <v-col>
        <v-select outlined dark solo :items="floors" label="Select Floor" height="50px"></v-select>
      </v-col>
      <v-col>
        <v-banner align="center">Sensor :</v-banner>
      </v-col>
      <v-col>
        <v-select outlined tile dark solo :items="sensors" label="Select Sensor" height="50px"></v-select>
      </v-col>
    </v-row>
    <v-row>
      <v-col height="420px" width="420px">
        <div style="background:#e8e7e3" class="small">
          <sensor-data-chart style="height:400px;" :chart-data="chartdata" />
        </div>
      </v-col>
    </v-row>
    <v-row grey>
      <!-- <label class="text-reader"> -->
      <!-- <input type="file" @change="loadTextFromFile" /> -->
      <v-col>
        <v-file-input
          v-model="file"
          label="File input"
          outlined
          dense
          filled
          counter
          @change="loadTextFromFile"
        ></v-file-input>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import SensorDataChart from "./../components/sensorDataChart.vue";

export default {
  data: () => ({
    floors: ["Floor 1", "Floor 2", "Floor 3"],
    sensors: ["Sensor 1", "Sensor 2", "Sensor 3"],
    loaded: false,
    chartdata: null,
    file: null
  }),
  components: {
    SensorDataChart
  },
  methods: {
    loadTextFromFile(ev) {
      // const file = ev.target.files[0];
      // const file = ev.target.files[0];
      const reader = new FileReader();
      var sensorValues = [];
      var sensorTimestamps = [];

      reader.onload = e => {
        let content = e.target.result;
        // this.$emit("load", e.target.result);
        // console.log(e.target.result);
        let txtLines = content.split("\n");
        txtLines.forEach(line => {
          let lineElements = line.split(":");
          sensorTimestamps.push(lineElements[0]);
          sensorValues.push(lineElements[1]);
        });
        this.chartdata = {
          labels: sensorTimestamps,
          datasets: [
            {
              label: "Sensor 1",
              backgroundColor: "#f87979",
              data: sensorValues
            }
          ]
        };
      };
      try {
        reader.readAsText(this.file);
      } catch (error) {
        console.log(error);
      }
    }
  }
};
</script>

<style>
  .small {
    max-width: 450px;
    max-height: 450px;
    margin:  50px auto;
  }
</style>