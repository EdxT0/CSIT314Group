export default function PlatformManagerPage() {
  return (
    <div style={styles.page}>
      <h1 style={styles.heading}>Platform Manager Dashboard</h1>
    </div>
  );
}

const styles = {
  page: {
    height: "100vh",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#f9fafb",
  },
  heading: {
    fontSize: "32px",
    fontWeight: "bold",
    color: "black",
  },
};